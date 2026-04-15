using Microsoft.AspNetCore.Identity;
using BuckeyeMarketplaceAPI.Models;

namespace BuckeyeMarketplaceAPI.Data
{
    /// <summary>
    /// Utility class to seed the database with test data
    /// </summary>
    public static class DbSeeder
    {
        /// <summary>
        /// Seeds the database with test data if it's empty
        /// </summary>
        public static async Task SeedAsync(AppDbContext context, IServiceProvider serviceProvider)
        {
            try
            {
                // Always seed roles and admin user first (Identity tables may be new)
                await SeedRolesAndAdminAsync(serviceProvider);

                // Only seed products/carts if database is empty
                if (context.Products.Any())
                {
                    Console.WriteLine("Product data already seeded. Skipping.");
                    return;
                }

                Console.WriteLine("Starting database seeding...");

                // Seed Products
                var products = new List<Product>
                {
                    new Product
                    {
                        Id = 1,
                        Title = "Introduction to Information Systems Textbook",
                        Description = "Comprehensive textbook covering IS fundamentals, business IT, and digital transformation. Perfect for AMIS 2100 course.",
                        Category = "Textbooks",
                        Price = 89.99m,
                        SellerName = "John Smith",
                        PostedDate = new DateTime(2026, 2, 15),
                        ImageUrl = "https://m.media-amazon.com/images/I/61wDhC8nNGL._AC_UF1000,1000_QL80_.jpg",
                        StockQuantity = 15
                    },
                    new Product
                    {
                        Id = 2,
                        Title = "Ohio State Sweatshirt - Medium",
                        Description = "Official Ohio State University sweatshirt in scarlet red. Comfortable and warm, perfect for game days!",
                        Category = "Apparel",
                        Price = 34.99m,
                        SellerName = "Emma Johnson",
                        PostedDate = new DateTime(2026, 2, 20),
                        ImageUrl = "https://www.osuonlinestore.com/product-images/49D09FF5-28CC-4A2C-BC7C-FA0E6FB4C06F/800x1000-new.jpg",
                        StockQuantity = 25
                    },
                    new Product
                    {
                        Id = 3,
                        Title = "Organic Chemistry Textbook - 8th Edition",
                        Description = "Advanced organic chemistry reference with detailed mechanisms and practice problems. Essential for CHEM 3210.",
                        Category = "Textbooks",
                        Price = 129.99m,
                        SellerName = "Alex Martinez",
                        PostedDate = new DateTime(2026, 2, 18),
                        ImageUrl = "https://images-na.ssl-images-amazon.com/images/P/B076W9V9GB.01.L.jpg",
                        StockQuantity = 0
                    },
                    new Product
                    {
                        Id = 4,
                        Title = "LED Desk Lamp - Adjustable",
                        Description = "Modern LED desk lamp with adjustable brightness and color temperature. Perfect for studying late nights.",
                        Category = "Electronics",
                        Price = 24.99m,
                        SellerName = "Sarah Lee",
                        PostedDate = new DateTime(2026, 2, 25),
                        ImageUrl = "https://encrypted-tbn2.gstatic.com/shopping?q=tbn:ANd9GcRQDvUa6IAUeyY01b9MIIP3a8rZcNZQq-kVNR0ylUQpERW9KsT7UtDHQD5Hab91d8Q-JZGCopcfrnPiXptjLaRR2qXx1KHXM5RnAbyR-nYfEB7r_4Hyhxt0rA",
                        StockQuantity = 30
                    },
                    new Product
                    {
                        Id = 5,
                        Title = "Winter Coat - Black XL",
                        Description = "Warm winter coat with waterproof exterior and fleece lining. Great condition, barely worn.",
                        Category = "Apparel",
                        Price = 99.99m,
                        SellerName = "Michael Brown",
                        PostedDate = new DateTime(2026, 2, 22),
                        ImageUrl = "https://images.unsplash.com/photo-1539533057440-7a9c185d69bb?w=500&h=500",
                        StockQuantity = 8
                    },
                    new Product
                    {
                        Id = 6,
                        Title = "55-inch 4K Smart TV",
                        Description = "55-inch 4K Smart TV with HDR support and built-in streaming apps. Perfect for movies and gaming.",
                        Category = "Electronics",
                        Price = 399.99m,
                        SellerName = "David Wilson",
                        PostedDate = new DateTime(2026, 2, 28),
                        ImageUrl = "https://images.unsplash.com/photo-1505355324368-082d36acaaf0?w=500&h=500",
                        StockQuantity = 5
                    },
                    new Product
                    {
                        Id = 7,
                        Title = "Compact Mini Refrigerator",
                        Description = "Perfect mini fridge for dorm rooms or offices. Keeps drinks cold and has freezer compartment.",
                        Category = "Appliances",
                        Price = 79.99m,
                        SellerName = "Jessica Garcia",
                        PostedDate = new DateTime(2026, 3, 1),
                        ImageUrl = "https://images.unsplash.com/photo-1584568694244-14fbbc50d737?w=500&h=500",
                        StockQuantity = 18
                    },
                    new Product
                    {
                        Id = 8,
                        Title = "Ohio State Jacket - Official",
                        Description = "Official Ohio State University jacket with embroidered logo. Great for showing school spirit!",
                        Category = "Apparel",
                        Price = 64.99m,
                        SellerName = "Christopher Davis",
                        PostedDate = new DateTime(2026, 2, 26),
                        ImageUrl = "https://www.osuonlinestore.com/product-images/D4C5B7F8-2E3B-4F9A-9C2D-7A1B5E8F3C2A/800x1000-new.jpg",
                        StockQuantity = 20
                    },
                    new Product
                    {
                        Id = 9,
                        Title = "Large Wall Mirror - Wood Frame",
                        Description = "Elegant large wall mirror with solid wood frame. Great condition and adds style to any room.",
                        Category = "Home Decor",
                        Price = 44.99m,
                        SellerName = "Amanda Rodriguez",
                        PostedDate = new DateTime(2026, 3, 2),
                        ImageUrl = "https://images.unsplash.com/photo-1599643478518-a784e5dc4c8f?w=500&h=500",
                        StockQuantity = 10
                    },
                    new Product
                    {
                        Id = 10,
                        Title = "Wireless Bluetooth Speaker",
                        Description = "Portable wireless speaker with excellent sound quality and 12-hour battery life.",
                        Category = "Electronics",
                        Price = 49.99m,
                        SellerName = "Kevin Anderson",
                        PostedDate = new DateTime(2026, 3, 5),
                        ImageUrl = "https://images.unsplash.com/photo-1608043152269-423dbba4e7e1?w=500&h=500",
                        StockQuantity = 22
                    },
                    new Product
                    {
                        Id = 11,
                        Title = "Calculus Textbook - 12th Edition",
                        Description = "Comprehensive calculus textbook with clear explanations and practice problems. Used for MATH 1161.",
                        Category = "Textbooks",
                        Price = 119.99m,
                        SellerName = "Rachel Thompson",
                        PostedDate = new DateTime(2026, 3, 3),
                        ImageUrl = "https://images.unsplash.com/photo-1524995997946-a1c2e315a42f?w=500&h=500",
                        StockQuantity = 14
                    },
                    new Product
                    {
                        Id = 12,
                        Title = "Mechanical Keyboard - RGB",
                        Description = "High-quality mechanical keyboard with RGB lighting. Perfect for typing and gaming.",
                        Category = "Electronics",
                        Price = 89.99m,
                        SellerName = "Tyler Moore",
                        PostedDate = new DateTime(2026, 3, 4),
                        ImageUrl = "https://images.unsplash.com/photo-1587829191301-c5e84c1a68f0?w=500&h=500",
                        StockQuantity = 16
                    }
                };

                await context.Products.AddRangeAsync(products);
                await context.SaveChangesAsync();
                Console.WriteLine($"✓ Seeded {products.Count} products");

                // Seed test Cart with CartItems
                var testCart = new Cart
                {
                    UserId = "user-1",
                    Items = new List<CartItem>
                    {
                        new CartItem
                        {
                            ProductId = 2,
                            Quantity = 1,
                            Title = "Ohio State Sweatshirt - Medium",
                            Price = 34.99m,
                            ImageUrl = "https://www.osuonlinestore.com/product-images/49D09FF5-28CC-4A2C-BC7C-FA0E6FB4C06F/800x1000-new.jpg",
                            Category = "Apparel",
                            SellerName = "Emma Johnson"
                        },
                        new CartItem
                        {
                            ProductId = 4,
                            Quantity = 2,
                            Title = "LED Desk Lamp - Adjustable",
                            Price = 24.99m,
                            ImageUrl = "https://encrypted-tbn2.gstatic.com/shopping?q=tbn:ANd9GcRQDvUa6IAUeyY01b9MIIP3a8rZcNZQq-kVNR0ylUQpERW9KsT7UtDHQD5Hab91d8Q-JZGCopcfrnPiXptjLaRR2qXx1KHXM5RnAbyR-nYfEB7r_4Hyhxt0rA",
                            Category = "Electronics",
                            SellerName = "Sarah Lee"
                        }
                    }
                };

                await context.Carts.AddAsync(testCart);
                await context.SaveChangesAsync();
                Console.WriteLine($"✓ Seeded test cart with {testCart.Items.Count} items for user 'user-1'");

                Console.WriteLine("✓ Database seeding completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error seeding database: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Seeds roles and admin user — always runs regardless of product data
        /// </summary>
        private static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Create roles
            string[] roles = { "Admin", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    Console.WriteLine($"✓ Created '{role}' role");
                }
            }

            const string adminEmail = "admin@buckeyemarketplace.com";
            const string adminPassword = "Admin123";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Admin User",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    Console.WriteLine($"✓ Seeded admin user: {adminEmail} / {adminPassword}");
                }
                else
                {
                    Console.WriteLine($"✗ Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}
