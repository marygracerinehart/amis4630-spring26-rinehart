using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace BuckeyeMarketplaceAPI.Tests;

/// <summary>
/// Integration tests for OrderController.
/// Tests order placement, retrieval, and admin status updates.
/// </summary>
public class OrderControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private static readonly JsonSerializerOptions JsonOpts =
        new() { PropertyNameCaseInsensitive = true };

    public OrderControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    /// <summary>Helper: adds an item to the user's cart so an order can be placed.</summary>
    private async Task AddItemToCart(HttpClient client)
    {
        var payload = new { ProductId = 1, Quantity = 1 };
        await client.PostAsJsonAsync("/api/cart", payload);
    }

    // ───────────────────────────────────────────────────────────────
    // POST /api/orders — Place an order
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task PlaceOrder_WithCartItems_Returns201()
    {
        var client = _factory.CreateAuthenticatedClient("order-user-1", "User");

        // Arrange — add item to cart
        await AddItemToCart(client);

        // Act
        var orderPayload = new { ShippingAddress = "123 Test St, Columbus, OH 43210" };
        var response = await client.PostAsJsonAsync("/api/orders", orderPayload);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        Assert.True(doc.RootElement.GetProperty("id").GetInt32() > 0);
        Assert.Equal("Pending", doc.RootElement.GetProperty("status").GetString());
        Assert.True(doc.RootElement.GetProperty("totalAmount").GetDecimal() > 0);
    }

    [Fact]
    public async Task PlaceOrder_EmptyCart_Returns400()
    {
        var client = _factory.CreateAuthenticatedClient("order-user-empty-cart", "User");

        var orderPayload = new { ShippingAddress = "456 Test Ave" };
        var response = await client.PostAsJsonAsync("/api/orders", orderPayload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("empty", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task PlaceOrder_NoShippingAddress_Returns400()
    {
        var client = _factory.CreateAuthenticatedClient("order-user-no-addr", "User");

        await AddItemToCart(client);

        var orderPayload = new { ShippingAddress = "" };
        var response = await client.PostAsJsonAsync("/api/orders", orderPayload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PlaceOrder_Unauthenticated_Returns401()
    {
        var client = _factory.CreateClient(); // no auth

        var orderPayload = new { ShippingAddress = "123 Test St" };
        var response = await client.PostAsJsonAsync("/api/orders", orderPayload);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // ───────────────────────────────────────────────────────────────
    // GET /api/orders/mine — User's order history
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetMyOrders_ReturnsUserOrders()
    {
        var client = _factory.CreateAuthenticatedClient("order-user-mine", "User");

        // Place an order first
        await AddItemToCart(client);
        await client.PostAsJsonAsync("/api/orders", new { ShippingAddress = "789 Test Blvd" });

        // Act
        var response = await client.GetAsync("/api/orders/mine");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        Assert.True(doc.RootElement.GetArrayLength() >= 1);
    }

    [Fact]
    public async Task GetMyOrders_Unauthenticated_Returns401()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/orders/mine");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // ───────────────────────────────────────────────────────────────
    // GET /api/orders/{id} — Get specific order
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetOrder_OwnOrder_Returns200()
    {
        var client = _factory.CreateAuthenticatedClient("order-user-get", "User");

        // Place an order
        await AddItemToCart(client);
        var createResp = await client.PostAsJsonAsync("/api/orders", new { ShippingAddress = "100 Get St" });
        var createBody = await createResp.Content.ReadAsStringAsync();
        using var createDoc = JsonDocument.Parse(createBody);
        int orderId = createDoc.RootElement.GetProperty("id").GetInt32();

        // Act
        var response = await client.GetAsync($"/api/orders/{orderId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetOrder_NotFound_Returns404()
    {
        var client = _factory.CreateAuthenticatedClient("order-user-notfound", "User");
        var response = await client.GetAsync("/api/orders/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ───────────────────────────────────────────────────────────────
    // PUT /api/orders/{orderId}/status — Admin only
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateOrderStatus_AsAdmin_Returns200()
    {
        var userClient = _factory.CreateAuthenticatedClient("order-user-status", "User");

        // Place an order as a regular user
        await AddItemToCart(userClient);
        var createResp = await userClient.PostAsJsonAsync("/api/orders", new { ShippingAddress = "200 Status St" });
        var createBody = await createResp.Content.ReadAsStringAsync();
        using var createDoc = JsonDocument.Parse(createBody);
        int orderId = createDoc.RootElement.GetProperty("id").GetInt32();

        // Act — admin updates status
        var adminClient = _factory.CreateAdminClient();
        var response = await adminClient.PutAsJsonAsync($"/api/orders/{orderId}/status", new { Status = "Shipped" });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("Shipped", body);
    }

    [Fact]
    public async Task UpdateOrderStatus_InvalidStatus_Returns400()
    {
        var userClient = _factory.CreateAuthenticatedClient("order-user-invalid-status", "User");

        await AddItemToCart(userClient);
        var createResp = await userClient.PostAsJsonAsync("/api/orders", new { ShippingAddress = "300 Invalid St" });
        var createBody = await createResp.Content.ReadAsStringAsync();
        using var createDoc = JsonDocument.Parse(createBody);
        int orderId = createDoc.RootElement.GetProperty("id").GetInt32();

        var adminClient = _factory.CreateAdminClient();
        var response = await adminClient.PutAsJsonAsync($"/api/orders/{orderId}/status", new { Status = "Bogus" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateOrderStatus_AsRegularUser_Returns403()
    {
        var client = _factory.CreateAuthenticatedClient("order-user-forbidden", "User");
        var response = await client.PutAsJsonAsync("/api/orders/1/status", new { Status = "Shipped" });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateOrderStatus_NonexistentOrder_Returns404()
    {
        var adminClient = _factory.CreateAdminClient();
        var response = await adminClient.PutAsJsonAsync("/api/orders/99999/status", new { Status = "Shipped" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
