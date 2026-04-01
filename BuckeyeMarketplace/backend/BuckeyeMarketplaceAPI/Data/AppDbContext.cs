using Microsoft.EntityFrameworkCore;
using BuckeyeMarketplaceAPI.Models;

namespace BuckeyeMarketplaceAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Cart> Carts => Set<Cart>();
        public DbSet<CartItem> CartItems => Set<CartItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ----- Relationships -----

            // Cart → CartItems (one-to-many)
            modelBuilder.Entity<Cart>()
                .HasMany(c => c.Items)
                .WithOne(ci => ci.Cart)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            // CartItem → Product (many-to-one, no cascade delete so removing a
            // product doesn't silently destroy cart items)
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // ----- Indexes -----
            modelBuilder.Entity<Cart>()
                .HasIndex(c => c.UserId);

            // ----- Seed Data: Products -----
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Title = "Introduction to Information Systems textbook",
                    Description = "Comprehensive textbook for IS fundamentals",
                    Category = "Textbooks",
                    Price = 89.99m,
                    SellerName = "John Smith",
                    PostedDate = new DateTime(2026, 2, 15),
                    ImageUrl = "https://m.media-amazon.com/images/I/61wDhC8nNGL._AC_UF1000,1000_QL80_.jpg"
                },
                new Product
                {
                    Id = 2,
                    Title = "Ohio State Sweatshirt - Medium",
                    Description = "Official Ohio State University sweatshirt",
                    Category = "Apparel",
                    Price = 34.99m,
                    SellerName = "Emma Johnson",
                    PostedDate = new DateTime(2026, 2, 20),
                    ImageUrl = ""
                },
                new Product
                {
                    Id = 3,
                    Title = "Organic Chemistry Textbook",
                    Description = "Advanced organic chemistry reference",
                    Category = "Textbooks",
                    Price = 129.99m,
                    SellerName = "Alex Martinez",
                    PostedDate = new DateTime(2026, 2, 18),
                    ImageUrl = ""
                },
                new Product
                {
                    Id = 4,
                    Title = "Desk Lamp",
                    Description = "LED desk lamp with adjustable brightness",
                    Category = "Electronics",
                    Price = 24.99m,
                    SellerName = "Sarah Lee",
                    PostedDate = new DateTime(2026, 2, 25),
                    ImageUrl = "https://encrypted-tbn2.gstatic.com/shopping?q=tbn:ANd9GcRQDvUa6IAUeyY01b9MIIP3a8rZcNZQq-kVNR0ylUQpERW9KsT7UtDHQD5Hab91d8Q-JZGCopcfrnPiXptjLaRR2qXx1KHXM5RnAbyR-nYfEB7r_4Hyhxt0rA"
                },
                new Product
                {
                    Id = 5,
                    Title = "Winter Coat",
                    Description = "Warm winter coat with waterproof exterior",
                    Category = "Apparel",
                    Price = 99.99m,
                    SellerName = "Michael Brown",
                    PostedDate = new DateTime(2026, 2, 22),
                    ImageUrl = ""
                },
                new Product
                {
                    Id = 6,
                    Title = "TV",
                    Description = "55-inch 4K Smart TV",
                    Category = "Electronics",
                    Price = 399.99m,
                    SellerName = "David Wilson",
                    PostedDate = new DateTime(2026, 2, 28),
                    ImageUrl = ""
                },
                new Product
                {
                    Id = 7,
                    Title = "Mini Refrigerator",
                    Description = "Compact mini fridge for dorm or office",
                    Category = "Appliances",
                    Price = 79.99m,
                    SellerName = "Jessica Garcia",
                    PostedDate = new DateTime(2026, 3, 1),
                    ImageUrl = ""
                },
                new Product
                {
                    Id = 8,
                    Title = "Ohio State Jacket",
                    Description = "Official Ohio State University jacket",
                    Category = "Apparel",
                    Price = 64.99m,
                    SellerName = "Christopher Davis",
                    PostedDate = new DateTime(2026, 2, 26),
                    ImageUrl = ""
                },
                new Product
                {
                    Id = 9,
                    Title = "Mirror",
                    Description = "Large wall mirror with wooden frame",
                    Category = "Home Decor",
                    Price = 44.99m,
                    SellerName = "Amanda Rodriguez",
                    PostedDate = new DateTime(2026, 3, 2),
                    ImageUrl = ""
                }
            );
        }
    }
}
