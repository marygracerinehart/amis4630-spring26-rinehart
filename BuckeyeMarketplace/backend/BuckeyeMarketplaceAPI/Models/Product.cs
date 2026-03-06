namespace BuckeyeMarketplaceAPI.Models
{
    public class Product
    {
        public int Id { get; set; } = 0;
        public string? Title { get; set; } = null;
        public string? Description { get; set; } = null;
        public string? Category { get; set; } = null;
        public decimal Price { get; set; } = 0m;
        public string? SellerName { get; set; } = null;
        public DateTime PostedDate { get; set; } = DateTime.MinValue;
        public string? ImageUrl { get; set; } = null;
    }
}
