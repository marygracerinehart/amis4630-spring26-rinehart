using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BuckeyeMarketplaceAPI.Data;
using BuckeyeMarketplaceAPI.Models;

namespace BuckeyeMarketplaceAPI.Tests;

// ───────── Fake authentication handler ─────────────────────────────
// Allows integration tests to bypass real JWT validation and
// inject custom claims (userId, roles) via a simple header.

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "TestScheme";
    public const string UserIdHeader = "X-Test-UserId";
    public const string RolesHeader  = "X-Test-Roles";

    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // If the request doesn't carry the test user-id header, treat as anonymous
        if (!Request.Headers.ContainsKey(UserIdHeader))
            return Task.FromResult(AuthenticateResult.NoResult());

        var userId = Request.Headers[UserIdHeader].ToString();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Email, $"{userId}@test.com"),
        };

        // Optional roles header — comma-separated
        if (Request.Headers.ContainsKey(RolesHeader))
        {
            foreach (var role in Request.Headers[RolesHeader].ToString().Split(','))
                claims.Add(new Claim(ClaimTypes.Role, role.Trim()));
        }

        var identity  = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket    = new AuthenticationTicket(principal, SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

// ───────── Custom WebApplicationFactory ────────────────────────────

/// <summary>
/// Custom WebApplicationFactory that:
///   1. Replaces SQLite with an EF Core in-memory database.
///   2. Replaces JWT auth with a test authentication handler.
///   3. Seeds products so integration tests have data to work with.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = "TestDb_" + Guid.NewGuid();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Provide test configuration values the app needs (e.g. JWT settings)
        builder.UseSetting("Jwt:Key", "ThisIsAVeryLongTestSecretKeyForHMACSHA256AtLeast32Chars!");
        builder.UseSetting("Jwt:Issuer", "TestIssuer");
        builder.UseSetting("Jwt:Audience", "TestAudience");
        builder.UseSetting("Jwt:ExpirationInMinutes", "60");
        builder.UseSetting("Jwt:RefreshTokenExpirationInDays", "7");

        builder.ConfigureServices(services =>
        {
            // ── Remove real EF registrations ─────────────────────────
            var efDescriptors = services
                .Where(d => d.ServiceType.FullName != null &&
                            (d.ServiceType.FullName.Contains("EntityFrameworkCore") ||
                             d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                             d.ServiceType == typeof(AppDbContext)))
                .ToList();

            foreach (var d in efDescriptors)
                services.Remove(d);

            // ── Add in-memory database ───────────────────────────────
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(_dbName));
        });

        builder.ConfigureTestServices(services =>
        {
            // ── Replace authentication with the test handler ─────────
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                options.DefaultChallengeScheme    = TestAuthHandler.SchemeName;
                options.DefaultScheme             = TestAuthHandler.SchemeName;
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                TestAuthHandler.SchemeName, _ => { });
        });

        builder.ConfigureServices(services =>
        {
            // ── Seed data ────────────────────────────────────────────
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();

            // ── Seed Identity roles so Auth tests can assign roles ───
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            foreach (var roleName in new[] { "Admin", "User" })
            {
                if (!roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                    roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
            }

            if (!db.Products.Any())
            {
                db.Products.AddRange(
                    new Product
                    {
                        Id = 1,
                        Title = "Test Textbook",
                        Description = "A textbook for testing",
                        Category = "Textbooks",
                        Price = 29.99m,
                        SellerName = "Test Seller",
                        PostedDate = new DateTime(2026, 1, 1),
                        ImageUrl = "https://example.com/test.jpg",
                        StockQuantity = 50
                    },
                    new Product
                    {
                        Id = 2,
                        Title = "Test Sweatshirt",
                        Description = "A sweatshirt for testing",
                        Category = "Apparel",
                        Price = 34.99m,
                        SellerName = "Test Seller 2",
                        PostedDate = new DateTime(2026, 2, 1),
                        ImageUrl = "https://example.com/test2.jpg",
                        StockQuantity = 10
                    },
                    new Product
                    {
                        Id = 3,
                        Title = "Out of Stock Item",
                        Description = "This item has no stock",
                        Category = "Textbooks",
                        Price = 129.99m,
                        SellerName = "Test Seller 3",
                        PostedDate = new DateTime(2026, 3, 1),
                        ImageUrl = "https://example.com/test3.jpg",
                        StockQuantity = 0
                    }
                );
                db.SaveChanges();
            }
        });
    }

    // ── Helper: create a client pre-configured with auth headers ──────

    /// <summary>Creates an HttpClient that authenticates as the given user with the given roles.</summary>
    public HttpClient CreateAuthenticatedClient(string userId = "test-user-1", string roles = "User")
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Add(TestAuthHandler.UserIdHeader, userId);
        client.DefaultRequestHeaders.Add(TestAuthHandler.RolesHeader, roles);
        return client;
    }

    /// <summary>Creates an HttpClient that authenticates as an Admin user.</summary>
    public HttpClient CreateAdminClient(string userId = "admin-user-1")
    {
        return CreateAuthenticatedClient(userId, "Admin");
    }
}
