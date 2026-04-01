using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuckeyeMarketplaceAPI.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; } = 0;

        [Required]
        [MaxLength(200)]
        public string? Title { get; set; } = null;

        [MaxLength(1000)]
        public string? Description { get; set; } = null;

        [MaxLength(100)]
        public string? Category { get; set; } = null;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } = 0m;

        [MaxLength(100)]
        public string? SellerName { get; set; } = null;

        public DateTime PostedDate { get; set; } = DateTime.MinValue;

        public string? ImageUrl { get; set; } = null;

        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
        public int StockQuantity { get; set; } = 10;
    }
}
