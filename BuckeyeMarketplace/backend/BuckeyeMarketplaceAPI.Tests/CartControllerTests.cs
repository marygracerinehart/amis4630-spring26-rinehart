using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace BuckeyeMarketplaceAPI.Tests;

/// <summary>
/// Integration tests for CartController using WebApplicationFactory.
/// Each test gets a fresh in-memory database via CustomWebApplicationFactory.
/// </summary>
public class CartControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOpts =
        new() { PropertyNameCaseInsensitive = true };

    public CartControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    // ---------------------------------------------------------------
    // 1. GET /api/cart — should return 200 with a cart object
    // ---------------------------------------------------------------
    [Fact]
    public async Task GetCart_Returns200()
    {
        var response = await _client.GetAsync("/api/cart");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("items", body, StringComparison.OrdinalIgnoreCase);
    }

    // ---------------------------------------------------------------
    // 2. POST /api/cart — valid body returns 201 Created
    // ---------------------------------------------------------------
    [Fact]
    public async Task AddToCart_ValidProduct_Returns201()
    {
        var payload = new { ProductId = 1, Quantity = 2 };

        var response = await _client.PostAsJsonAsync("/api/cart", payload);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        Assert.Equal(1, doc.RootElement.GetProperty("productId").GetInt32());
        Assert.Equal(2, doc.RootElement.GetProperty("quantity").GetInt32());
    }

    // ---------------------------------------------------------------
    // 3. POST /api/cart — invalid productId returns 404
    // ---------------------------------------------------------------
    [Fact]
    public async Task AddToCart_InvalidProductId_Returns404()
    {
        var payload = new { ProductId = 9999, Quantity = 1 };

        var response = await _client.PostAsJsonAsync("/api/cart", payload);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ---------------------------------------------------------------
    // 3b. POST /api/cart — zero quantity returns 400
    // ---------------------------------------------------------------
    [Fact]
    public async Task AddToCart_ZeroQuantity_Returns400()
    {
        var payload = new { ProductId = 1, Quantity = 0 };

        var response = await _client.PostAsJsonAsync("/api/cart", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ---------------------------------------------------------------
    // 4. PUT /api/cart/{id} — updates quantity and returns 200
    // ---------------------------------------------------------------
    [Fact]
    public async Task UpdateCartItem_Returns200WithUpdatedQuantity()
    {
        // Arrange — add an item first
        var addPayload = new { ProductId = 1, Quantity = 1 };
        var addResponse = await _client.PostAsJsonAsync("/api/cart", addPayload);
        var addBody = await addResponse.Content.ReadAsStringAsync();
        using var addDoc = JsonDocument.Parse(addBody);
        int cartItemId = addDoc.RootElement.GetProperty("id").GetInt32();

        // Act — update its quantity
        var updatePayload = new { Quantity = 5 };
        var response = await _client.PutAsJsonAsync($"/api/cart/{cartItemId}", updatePayload);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        Assert.Equal(5, doc.RootElement.GetProperty("quantity").GetInt32());
    }

    // ---------------------------------------------------------------
    // 5. DELETE /api/cart/{id} — removes item and returns 200
    // ---------------------------------------------------------------
    [Fact]
    public async Task RemoveCartItem_Returns200()
    {
        // Arrange — add an item first
        var addPayload = new { ProductId = 1, Quantity = 1 };
        var addResponse = await _client.PostAsJsonAsync("/api/cart", addPayload);
        var addBody = await addResponse.Content.ReadAsStringAsync();
        using var addDoc = JsonDocument.Parse(addBody);
        int cartItemId = addDoc.RootElement.GetProperty("id").GetInt32();

        // Act — delete it
        var response = await _client.DeleteAsync($"/api/cart/{cartItemId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("removed", body, StringComparison.OrdinalIgnoreCase);
    }

    // ---------------------------------------------------------------
    // 6. DELETE /api/cart/clear — clears entire cart and returns 200
    // ---------------------------------------------------------------
    [Fact]
    public async Task ClearCart_Returns200()
    {
        // Arrange — add an item so there is something to clear
        var addPayload = new { ProductId = 1, Quantity = 1 };
        await _client.PostAsJsonAsync("/api/cart", addPayload);

        // Act
        var response = await _client.DeleteAsync("/api/cart/clear");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("cleared", body, StringComparison.OrdinalIgnoreCase);
    }
}
