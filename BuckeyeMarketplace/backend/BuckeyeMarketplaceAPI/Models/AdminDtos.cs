using System.ComponentModel.DataAnnotations;

namespace BuckeyeMarketplaceAPI.Models
{
    // ── User Management DTOs ──────────────────────────────────────

    /// <summary>Response DTO that represents a user in admin listings.</summary>
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? Bio { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> Roles { get; set; } = new();
    }

    /// <summary>Request DTO for assigning a role to a user.</summary>
    public class AssignRoleDto
    {
        [Required]
        public string Role { get; set; } = string.Empty;
    }

    // ── Order Management DTOs ─────────────────────────────────────

    /// <summary>Request DTO for updating an order's status.</summary>
    public class UpdateOrderStatusDto
    {
        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty;
    }

    // ── Dashboard DTO ─────────────────────────────────────────────

    /// <summary>Response DTO with high-level marketplace statistics.</summary>
    public class DashboardDto
    {
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int PendingOrders { get; set; }
    }
}
