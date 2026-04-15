using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace BuckeyeMarketplaceAPI.Tests;

/// <summary>
/// Single integration test for an authenticated endpoint using
/// WebApplicationFactory&lt;Program&gt; and the EF Core in-memory database.
///
/// Exercises the full checkout flow through real HTTP:
///   1. Add a product to the cart  (POST /api/cart)
///   2. Place an order             (POST /api/orders)
///   3. Retrieve order history     (GET  /api/orders/mine)
///   4. Assert the order matches what was placed.
/// </summary>
public class PlaceOrderIntegrationTest : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOpts =
        new() { PropertyNameCaseInsensitive = true };

    public PlaceOrderIntegrationTest(CustomWebApplicationFactory factory)
    {
        // Authenticated client — simulates a logged-in user via TestAuthHandler
        _client = factory.CreateAuthenticatedClient("integration-order-user");
    }

    [Fact]
    public async Task Checkout_AddToCart_PlaceOrder_AppearsInHistory()
    {
        // ── 1. Add a seeded product to the cart ──────────────────────
        var addResponse = await _client.PostAsJsonAsync("/api/cart", new
        {
            ProductId = 1,
            Quantity  = 2
        });
        Assert.Equal(HttpStatusCode.Created, addResponse.StatusCode);

        // Read back what the cart captured for product 1
        var cartBody = await addResponse.Content.ReadAsStringAsync();
        using var cartDoc = JsonDocument.Parse(cartBody);
        var cartTitle = cartDoc.RootElement.GetProperty("title").GetString();
        var cartPrice = cartDoc.RootElement.GetProperty("price").GetDecimal();

        // ── 2. Place the order ───────────────────────────────────────
        var orderResponse = await _client.PostAsJsonAsync("/api/orders", new
        {
            ShippingAddress = "123 Buckeye Lane, Columbus, OH 43210"
        });
        Assert.Equal(HttpStatusCode.Created, orderResponse.StatusCode);

        var orderBody = await orderResponse.Content.ReadAsStringAsync();
        using var orderDoc = JsonDocument.Parse(orderBody);

        var orderId = orderDoc.RootElement.GetProperty("id").GetInt32();
        var status  = orderDoc.RootElement.GetProperty("status").GetString();
        var total   = orderDoc.RootElement.GetProperty("totalAmount").GetDecimal();
        var items   = orderDoc.RootElement.GetProperty("items");

        Assert.True(orderId > 0);
        Assert.Equal("Pending", status);
        Assert.Equal(1, items.GetArrayLength());

        // The order item should carry over the same snapshot the cart captured
        var firstItem  = items[0];
        Assert.Equal(cartTitle, firstItem.GetProperty("title").GetString());
        Assert.Equal(cartPrice, firstItem.GetProperty("unitPrice").GetDecimal());
        Assert.Equal(2, firstItem.GetProperty("quantity").GetInt32());
        Assert.Equal(cartPrice * 2, total);

        // ── 3. Retrieve order history and verify the order is there ──
        var historyResponse = await _client.GetAsync("/api/orders/mine");
        Assert.Equal(HttpStatusCode.OK, historyResponse.StatusCode);

        var historyBody = await historyResponse.Content.ReadAsStringAsync();
        using var historyDoc = JsonDocument.Parse(historyBody);

        var orders = historyDoc.RootElement;
        Assert.True(orders.GetArrayLength() >= 1, "Order history should contain at least one order.");

        var found = false;
        foreach (var o in orders.EnumerateArray())
        {
            if (o.GetProperty("id").GetInt32() == orderId)
            {
                Assert.Equal("Pending", o.GetProperty("status").GetString());
                Assert.Equal(cartPrice * 2, o.GetProperty("totalAmount").GetDecimal());
                Assert.Contains("Buckeye Lane", o.GetProperty("shippingAddress").GetString());
                found = true;
                break;
            }
        }
        Assert.True(found, $"Order {orderId} should appear in the user's order history.");
    }
}
