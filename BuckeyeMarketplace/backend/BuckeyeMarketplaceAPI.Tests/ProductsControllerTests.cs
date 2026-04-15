using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace BuckeyeMarketplaceAPI.Tests;

/// <summary>
/// Integration tests for ProductsController.
/// GET endpoints are public; POST/PUT/DELETE require Admin role.
/// </summary>
public class ProductsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private static readonly JsonSerializerOptions JsonOpts =
        new() { PropertyNameCaseInsensitive = true };

    public ProductsControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    // ───────────────────────────────────────────────────────────────
    // GET /api/products — Public endpoint
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetProducts_Returns200WithList()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/products");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        Assert.True(doc.RootElement.GetArrayLength() > 0, "Products list should not be empty.");
    }

    // ───────────────────────────────────────────────────────────────
    // GET /api/products/{id} — Public endpoint
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetProduct_ValidId_Returns200()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/products/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        Assert.Equal(1, doc.RootElement.GetProperty("id").GetInt32());
    }

    [Fact]
    public async Task GetProduct_InvalidId_Returns404()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/products/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ───────────────────────────────────────────────────────────────
    // POST /api/products — Requires Admin role
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateProduct_AsAdmin_Returns201()
    {
        var client = _factory.CreateAdminClient();

        var newProduct = new
        {
            Id = 100,
            Title = "New Test Product",
            Description = "Created in test",
            Category = "Textbooks",
            Price = 19.99m,
            SellerName = "Test Admin",
            PostedDate = "2026-04-01",
            ImageUrl = "https://example.com/new.jpg",
            StockQuantity = 5
        };

        var response = await client.PostAsJsonAsync("/api/products", newProduct);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        Assert.Equal("New Test Product", doc.RootElement.GetProperty("title").GetString());
    }

    [Fact]
    public async Task CreateProduct_AsRegularUser_Returns403()
    {
        var client = _factory.CreateAuthenticatedClient("regular-user", "User");

        var newProduct = new
        {
            Id = 101,
            Title = "Forbidden Product",
            Description = "Should not be created",
            Category = "Textbooks",
            Price = 9.99m,
            SellerName = "Regular User",
            PostedDate = "2026-04-01",
            ImageUrl = "https://example.com/forbidden.jpg",
            StockQuantity = 1
        };

        var response = await client.PostAsJsonAsync("/api/products", newProduct);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_Unauthenticated_Returns401()
    {
        var client = _factory.CreateClient(); // no auth headers

        var newProduct = new
        {
            Id = 102,
            Title = "Unauthenticated Product",
            Description = "Should not be created",
            Category = "Textbooks",
            Price = 9.99m,
            SellerName = "Nobody",
            PostedDate = "2026-04-01",
            ImageUrl = "https://example.com/unauth.jpg",
            StockQuantity = 1
        };

        var response = await client.PostAsJsonAsync("/api/products", newProduct);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // ───────────────────────────────────────────────────────────────
    // DELETE /api/products/{id} — Requires Admin role
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteProduct_AsRegularUser_Returns403()
    {
        var client = _factory.CreateAuthenticatedClient("regular-user-2", "User");
        var response = await client.DeleteAsync("/api/products/1");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_InvalidId_Returns404()
    {
        var client = _factory.CreateAdminClient();
        var response = await client.DeleteAsync("/api/products/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
