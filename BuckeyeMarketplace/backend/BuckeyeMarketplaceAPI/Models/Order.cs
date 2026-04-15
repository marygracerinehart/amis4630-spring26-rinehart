using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuckeyeMarketplaceAPI.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(450)]
        public string UserId { get; set; } = string.Empty;

        /// <summary>Navigation property to the Identity user who placed the order</summary>
        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending";

        /// <summary>Total price of the order (sum of all line items)</summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        /// <summary>Shipping address provided at checkout</summary>
        [Required]
        [MaxLength(500)]
        public string ShippingAddress { get; set; } = string.Empty;

        /// <summary>Navigation collection — the line items in this order</summary>
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
