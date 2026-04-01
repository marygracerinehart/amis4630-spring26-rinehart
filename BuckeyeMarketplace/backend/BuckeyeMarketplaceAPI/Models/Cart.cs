using System.ComponentModel.DataAnnotations;

namespace BuckeyeMarketplaceAPI.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string UserId { get; set; } = string.Empty;

        /// <summary>Navigation collection — the line items in this cart</summary>
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
