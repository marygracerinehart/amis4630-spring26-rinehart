using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BuckeyeMarketplaceAPI.Data;
using BuckeyeMarketplaceAPI.Models;

namespace BuckeyeMarketplaceAPI.Tests;

/// <summary>
/// Custom WebApplicationFactory that replaces SQLite with an EF Core
/// in-memory database and seeds one product so cart tests have something
/// to reference.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = "TestCartDb_" + Guid.NewGuid();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove every EF / DbContext descriptor that the real app registered
            // so we don't end up with two database providers.
            var efDescriptors = services
                .Where(d => d.ServiceType.FullName != null &&
                            (d.ServiceType.FullName.Contains("EntityFrameworkCore") ||
                             d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                             d.ServiceType == typeof(AppDbContext)))
                .ToList();

            foreach (var d in efDescriptors)
                services.Remove(d);

            // Add an in-memory database for testing (single name for all scopes)
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(_dbName));
        });

        builder.ConfigureServices(services =>
        {
            // Build a temporary provider to seed the shared in-memory store
            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();

            // Seed a product the tests can add to the cart
            if (!db.Products.Any())
            {
                db.Products.Add(new Product
                {
                    Id = 1,
                    Title = "Test Textbook",
                    Description = "A textbook for testing",
                    Category = "Textbooks",
                    Price = 29.99m,
                    SellerName = "Test Seller",
                    PostedDate = new DateTime(2026, 1, 1),
                    ImageUrl = "https://example.com/test.jpg"
                });
                db.SaveChanges();
            }
        });
    }
}
