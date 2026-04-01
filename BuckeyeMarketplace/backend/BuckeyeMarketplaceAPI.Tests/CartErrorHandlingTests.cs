using Xunit;
using System.Net;
using System.Net.Http.Json;

namespace BuckeyeMarketplaceAPI.Tests;

/// <summary>
/// Tests to verify frontend loading and error states work correctly
/// </summary>
public class CartErrorHandlingTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CartErrorHandlingTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    // ---------------------------------------------------------------
    // Test 1: Invalid Product ID returns 404
    // Frontend should display error message
    // ---------------------------------------------------------------
    [Fact]
    public async Task AddToCart_InvalidProductId_Returns404()
    {
        var payload = new { ProductId = 9999, Quantity = 1 };
        var response = await _client.PostAsJsonAsync("/api/cart", payload);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("not found", body, StringComparison.OrdinalIgnoreCase);
    }

    // ---------------------------------------------------------------
    // Test 2: Zero quantity returns 400
    // Frontend should display error message
    // ---------------------------------------------------------------
    [Fact]
    public async Task AddToCart_ZeroQuantity_Returns400()
    {
        var payload = new { ProductId = 1, Quantity = 0 };
        var response = await _client.PostAsJsonAsync("/api/cart", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("greater than zero", body, StringComparison.OrdinalIgnoreCase);
    }

    // ---------------------------------------------------------------
    // Test 3: Invalid cart item ID returns 404 on update
    // Frontend should display error message
    // ---------------------------------------------------------------
    [Fact]
    public async Task UpdateCartItem_InvalidItemId_Returns404()
    {
        var payload = new { Quantity = 5 };
        var response = await _client.PutAsJsonAsync("/api/cart/9999", payload);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("not found", body, StringComparison.OrdinalIgnoreCase);
    }

    // ---------------------------------------------------------------
    // Test 4: Invalid cart item ID returns 404 on delete
    // Frontend should display error message
    // ---------------------------------------------------------------
    [Fact]
    public async Task RemoveCartItem_InvalidItemId_Returns404()
    {
        var response = await _client.DeleteAsync("/api/cart/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("not found", body, StringComparison.OrdinalIgnoreCase);
    }

    // ---------------------------------------------------------------
    // Test 5: Invalid quantity on update returns 400
    // Frontend should display error message
    // ---------------------------------------------------------------
    [Fact]
    public async Task UpdateCartItem_InvalidQuantity_Returns400()
    {
        // Add item first
        var addPayload = new { ProductId = 1, Quantity = 1 };
        var addResponse = await _client.PostAsJsonAsync("/api/cart", addPayload);
        var addBody = await addResponse.Content.ReadAsStringAsync();
        using var addDoc = System.Text.Json.JsonDocument.Parse(addBody);
        int cartItemId = addDoc.RootElement.GetProperty("id").GetInt32();

        // Try to update with invalid quantity
        var updatePayload = new { Quantity = 0 };
        var response = await _client.PutAsJsonAsync($"/api/cart/{cartItemId}", updatePayload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("greater than zero", body, StringComparison.OrdinalIgnoreCase);
    }

    // ---------------------------------------------------------------
    // Test 6: Successful cart retrieval with 200 OK
    // Frontend should clear loading and error states
    // ---------------------------------------------------------------
    [Fact]
    public async Task GetCart_Returns200WithValidData()
    {
        var response = await _client.GetAsync("/api/cart");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(body);
        Assert.Contains("items", body, StringComparison.OrdinalIgnoreCase);
    }

    // ---------------------------------------------------------------
    // Test 7: Successful add returns 201 or 200
    // Frontend should clear loading and error states
    // ---------------------------------------------------------------
    [Fact]
    public async Task AddToCart_ValidProduct_ReturnsSuccess()
    {
        var payload = new { ProductId = 1, Quantity = 1 };
        var response = await _client.PostAsJsonAsync("/api/cart", payload);

        // Accept both 201 Created and 200 OK
        Assert.True(response.StatusCode == HttpStatusCode.Created || 
                    response.StatusCode == HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(body);
        Assert.Contains("id", body, StringComparison.OrdinalIgnoreCase);
    }
}
