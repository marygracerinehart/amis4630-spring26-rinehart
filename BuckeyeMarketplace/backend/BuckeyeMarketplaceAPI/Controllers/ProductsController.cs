using Microsoft.AspNetCore.Mvc;
using BuckeyeMarketplaceAPI.Models;

namespace BuckeyeMarketplaceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private static List<Product> products = new List<Product>
        {
            new Product { Id = 1, Title = "Introduction to Information Systems textbook", Description = "Comprehensive textbook for IS fundamentals", Category = "Textbooks", Price = 89.99m, SellerName = "John Smith", PostedDate = new DateTime(2026, 2, 15), ImageUrl = "https://m.media-amazon.com/images/I/61wDhC8nNGL._AC_UF1000,1000_QL80_.jpg" },
            new Product { Id = 2, Title = "Ohio State Sweatshirt - Medium", Description = "Official Ohio State University sweatshirt", Category = "Apparel", Price = 34.99m, SellerName = "Emma Johnson", PostedDate = new DateTime(2026, 2, 20), ImageUrl = "https://images.unsplash.com/photo-1556821552-5eb1d3c7a30e?w=500&h=500&fit=crop" },
            new Product { Id = 3, Title = "Organic Chemistry Textbook", Description = "Advanced organic chemistry reference", Category = "Textbooks", Price = 129.99m, SellerName = "Alex Martinez", PostedDate = new DateTime(2026, 2, 18), ImageUrl = "https://images.unsplash.com/photo-1507842872343-583f20270319?w=500&h=500&fit=crop" },
            new Product { Id = 4, Title = "Desk Lamp", Description = "LED desk lamp with adjustable brightness", Category = "Electronics", Price = 24.99m, SellerName = "Sarah Lee", PostedDate = new DateTime(2026, 2, 25), ImageUrl = "https://images.unsplash.com/photo-1565636192335-14f0c6b03c29?w=500&h=500&fit=crop" },
            new Product { Id = 5, Title = "Winter Coat", Description = "Warm winter coat with waterproof exterior", Category = "Apparel", Price = 99.99m, SellerName = "Michael Brown", PostedDate = new DateTime(2026, 2, 22), ImageUrl = "https://images.unsplash.com/photo-1539533057687-c8a8fc3f8afb?w=500&h=500&fit=crop" },
            new Product { Id = 6, Title = "TV", Description = "55-inch 4K Smart TV", Category = "Electronics", Price = 399.99m, SellerName = "David Wilson", PostedDate = new DateTime(2026, 2, 28), ImageUrl = "https://images.unsplash.com/photo-1461896836934-ffe607ba8211?w=500&h=500&fit=crop" },
            new Product { Id = 7, Title = "Mini Refrigerator", Description = "Compact mini fridge for dorm or office", Category = "Appliances", Price = 79.99m, SellerName = "Jessica Garcia", PostedDate = new DateTime(2026, 3, 1), ImageUrl = "https://images.unsplash.com/photo-1584568694244-14fbbc50d737?w=500&h=500&fit=crop" },
            new Product { Id = 8, Title = "Ohio State Jacket", Description = "Official Ohio State University jacket", Category = "Apparel", Price = 64.99m, SellerName = "Christopher Davis", PostedDate = new DateTime(2026, 2, 26), ImageUrl = "https://images.unsplash.com/photo-1551028719-00167b16ebc5?w=500&h=500&fit=crop" },
            new Product { Id = 9, Title = "Mirror", Description = "Large wall mirror with wooden frame", Category = "Home Decor", Price = 44.99m, SellerName = "Amanda Rodriguez", PostedDate = new DateTime(2026, 3, 2), ImageUrl = "https://images.unsplash.com/photo-1556909114-f6e7ad7d3136?w=500&h=500&fit=crop" }
        };

        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetProducts()
        {
            return Ok(products);
        }

        [HttpGet("{id}")]
        public ActionResult<Product> GetProduct(int id)
        {
            var product = products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public ActionResult<Product> CreateProduct(Product product)
        {
            products.Add(product);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, Product product)
        {
            var existingProduct = products.FirstOrDefault(p => p.Id == id);
            if (existingProduct == null)
                return NotFound();

            existingProduct.Title = product.Title;
            existingProduct.Description = product.Description;
            existingProduct.Category = product.Category;
            existingProduct.Price = product.Price;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var product = products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound();

            products.Remove(product);
            return NoContent();
        }
    }
}
