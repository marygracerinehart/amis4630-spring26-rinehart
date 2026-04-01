using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BuckeyeMarketplaceAPI.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        /// <summary>Foreign key to Cart</summary>
        [Required]
        public int CartId { get; set; }

        /// <summary>Navigation property back to the parent Cart</summary>
        [JsonIgnore]
        [ForeignKey(nameof(CartId))]
        public Cart Cart { get; set; } = null!;

        /// <summary>Foreign key to Product</summary>
        [Required]
        public int ProductId { get; set; }

        /// <summary>Navigation property to the related Product</summary>
        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        // Denormalized product fields — captured at the time the item is added
        // so the cart retains the price/title even if the product changes later.
        [Required]
        [MaxLength(200)]
        public string? Title { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        [MaxLength(100)]
        public string? Category { get; set; }

        [MaxLength(100)]
        public string? SellerName { get; set; }
    }
}
