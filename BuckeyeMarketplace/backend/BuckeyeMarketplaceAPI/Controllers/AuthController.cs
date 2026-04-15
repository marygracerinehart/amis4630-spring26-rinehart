using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BuckeyeMarketplaceAPI.Data;
using BuckeyeMarketplaceAPI.Models;

namespace BuckeyeMarketplaceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _context = context;
        }

        /// <summary>
        /// Register a new user account.
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
                return BadRequest(new { message = "A user with this email already exists." });

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { message = "Registration failed.", errors });
            }

            // Assign default "User" role
            await _userManager.AddToRoleAsync(user, "User");

            return Ok(await GenerateAuthResponse(user));
        }

        /// <summary>
        /// Log in with an existing account.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized(new { message = "Invalid email or password." });

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);

            if (!result.Succeeded)
                return Unauthorized(new { message = "Invalid email or password." });

            return Ok(await GenerateAuthResponse(user));
        }

        /// <summary>
        /// Exchange an expired access token + valid refresh token for a new token pair.
        /// </summary>
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validate the expired access token to extract the user identity
            var principal = GetPrincipalFromExpiredToken(model.Token);
            if (principal == null)
                return Unauthorized(new { message = "Invalid access token." });

            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (userId == null)
                return Unauthorized(new { message = "Invalid access token." });

            // Look up the refresh token in the database
            var storedToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == model.RefreshToken && rt.UserId == userId);

            if (storedToken == null || !storedToken.IsActive)
                return Unauthorized(new { message = "Invalid or expired refresh token." });

            // Revoke the old refresh token (rotation)
            storedToken.RevokedAt = DateTime.UtcNow;
            _context.RefreshTokens.Update(storedToken);

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Unauthorized(new { message = "User not found." });

            var response = await GenerateAuthResponse(user);
            return Ok(response);
        }

        /// <summary>
        /// Revoke a refresh token so it can no longer be used.
        /// </summary>
        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke([FromBody] RefreshTokenRequestDto model)
        {
            var storedToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == model.RefreshToken);

            if (storedToken == null || !storedToken.IsActive)
                return BadRequest(new { message = "Token is already revoked or does not exist." });

            storedToken.RevokedAt = DateTime.UtcNow;
            _context.RefreshTokens.Update(storedToken);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Refresh token revoked successfully." });
        }

        // ───────── Private helpers ─────────

        private async Task<AuthResponseDto> GenerateAuthResponse(ApplicationUser user)
        {
            var accessToken = await GenerateJwtToken(user);
            var refreshToken = await GenerateRefreshToken(user.Id);
            var expiration = DateTime.UtcNow.AddMinutes(
                Convert.ToDouble(_configuration["Jwt:ExpirationInMinutes"]));
            var roles = await _userManager.GetRolesAsync(user);

            return new AuthResponseDto
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                Expiration = expiration,
                UserId = user.Id,
                Email = user.Email!,
                FullName = user.FullName ?? string.Empty,
                Roles = roles.ToList()
            };
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var jwtKey = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT Key is not configured.");
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtAudience = _configuration["Jwt:Audience"];
            var expMinutes = Convert.ToDouble(_configuration["Jwt:ExpirationInMinutes"]);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Build claims including roles
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("fullName", user.FullName ?? string.Empty)
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<string> GenerateRefreshToken(string userId)
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            var tokenString = Convert.ToBase64String(randomBytes);

            var refreshDays = Convert.ToDouble(
                _configuration["Jwt:RefreshTokenExpirationInDays"] ?? "7");

            var refreshToken = new RefreshToken
            {
                Token = tokenString,
                UserId = userId,
                ExpiresAt = DateTime.UtcNow.AddDays(refreshDays),
                CreatedAt = DateTime.UtcNow
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return tokenString;
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var jwtKey = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT Key is not configured.");

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = false, // allow expired tokens
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
                if (securityToken is not JwtSecurityToken jwtToken ||
                    !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    return null;

                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
