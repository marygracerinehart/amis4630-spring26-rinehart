using Microsoft.AspNetCore.Identity;

namespace BuckeyeMarketplaceAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        /// <summary>User's full name</summary>
        public string? FullName { get; set; }

        /// <summary>User's bio or description</summary>
        public string? Bio { get; set; }

        /// <summary>Account creation date</summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
