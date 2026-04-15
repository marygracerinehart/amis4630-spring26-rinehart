using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BuckeyeMarketplaceAPI.Data;
using BuckeyeMarketplaceAPI.Models;

namespace BuckeyeMarketplaceAPI.Controllers
{
    /// <summary>
    /// Admin-only endpoints for managing users, orders, and viewing dashboard stats.
    /// Every endpoint in this controller requires the "Admin" role.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // ══════════════════════════════════════════════════════════════
        //  USER MANAGEMENT
        // ══════════════════════════════════════════════════════════════

        /// <summary>
        /// GET /api/admin/users — List all registered users with their roles.
        /// </summary>
        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    FullName = user.FullName,
                    Bio = user.Bio,
                    CreatedAt = user.CreatedAt,
                    Roles = roles.ToList()
                });
            }

            return Ok(userDtos);
        }

        /// <summary>
        /// GET /api/admin/users/{id} — Get a single user's details by ID.
        /// </summary>
        [HttpGet("users/{id}")]
        public async Task<ActionResult<UserDto>> GetUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { message = $"User with ID '{id}' not found." });

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new UserDto
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName,
                Bio = user.Bio,
                CreatedAt = user.CreatedAt,
                Roles = roles.ToList()
            });
        }

        /// <summary>
        /// POST /api/admin/users/{id}/role — Assign a role to a user.
        /// </summary>
        [HttpPost("users/{id}/role")]
        public async Task<IActionResult> AssignRole(string id, [FromBody] AssignRoleDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { message = $"User with ID '{id}' not found." });

            // Make sure the role exists
            if (!await _roleManager.RoleExistsAsync(model.Role))
                return BadRequest(new { message = $"Role '{model.Role}' does not exist." });

            // Don't add duplicate roles
            if (await _userManager.IsInRoleAsync(user, model.Role))
                return BadRequest(new { message = $"User already has the '{model.Role}' role." });

            var result = await _userManager.AddToRoleAsync(user, model.Role);
            if (!result.Succeeded)
                return BadRequest(new { message = "Failed to assign role.", errors = result.Errors });

            return Ok(new { message = $"Role '{model.Role}' assigned to user '{user.Email}'." });
        }

        /// <summary>
        /// DELETE /api/admin/users/{id}/role — Remove a role from a user.
        /// </summary>
        [HttpDelete("users/{id}/role/{role}")]
        public async Task<IActionResult> RemoveRole(string id, string role)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { message = $"User with ID '{id}' not found." });

            if (!await _userManager.IsInRoleAsync(user, role))
                return BadRequest(new { message = $"User does not have the '{role}' role." });

            var result = await _userManager.RemoveFromRoleAsync(user, role);
            if (!result.Succeeded)
                return BadRequest(new { message = "Failed to remove role.", errors = result.Errors });

            return Ok(new { message = $"Role '{role}' removed from user '{user.Email}'." });
        }

        /// <summary>
        /// DELETE /api/admin/users/{id} — Delete a user account.
        /// </summary>
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { message = $"User with ID '{id}' not found." });

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest(new { message = "Failed to delete user.", errors = result.Errors });

            return Ok(new { message = $"User '{user.Email}' deleted successfully." });
        }

        // ══════════════════════════════════════════════════════════════
        //  ORDER MANAGEMENT
        // ══════════════════════════════════════════════════════════════

        /// <summary>
        /// GET /api/admin/orders — List ALL orders across all users.
        /// </summary>
        [HttpGet("orders")]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.Items)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return Ok(orders);
        }

        /// <summary>
        /// GET /api/admin/orders/{id} — Get any order by ID (not restricted to the current user).
        /// </summary>
        [HttpGet("orders/{id}")]
        public async Task<ActionResult<Order>> GetOrderById(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound(new { message = $"Order with ID {id} not found." });

            return Ok(order);
        }

        /// <summary>
        /// PUT /api/admin/orders/{id}/status — Update an order's status (e.g. Pending → Shipped → Delivered).
        /// </summary>
        [HttpPut("orders/{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound(new { message = $"Order with ID {id} not found." });

            order.Status = model.Status;
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Order {id} status updated to '{model.Status}'." });
        }

        // ══════════════════════════════════════════════════════════════
        //  DASHBOARD / STATS
        // ══════════════════════════════════════════════════════════════

        /// <summary>
        /// GET /api/admin/dashboard — Get high-level marketplace statistics.
        /// </summary>
        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardDto>> GetDashboard()
        {
            var totalUsers = await _userManager.Users.CountAsync();
            var totalProducts = await _context.Products.CountAsync();
            var totalOrders = await _context.Orders.CountAsync();
            var totalRevenue = await _context.Orders.SumAsync(o => o.TotalAmount);
            var pendingOrders = await _context.Orders.CountAsync(o => o.Status == "Pending");

            return Ok(new DashboardDto
            {
                TotalUsers = totalUsers,
                TotalProducts = totalProducts,
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                PendingOrders = pendingOrders
            });
        }
    }
}
