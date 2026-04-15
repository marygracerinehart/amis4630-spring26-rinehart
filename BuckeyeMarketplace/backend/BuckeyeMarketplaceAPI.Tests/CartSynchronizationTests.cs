using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace BuckeyeMarketplaceAPI.Tests;

/// <summary>
/// Integration tests for cart state synchronization between frontend and backend.
/// These tests verify that:
/// 1. Cart state persists in the database
/// 2. Frontend and backend stay in sync after each operation
/// 3. Multiple operations maintain consistency
/// </summary>
public class CartSynchronizationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOpts =
        new() { PropertyNameCaseInsensitive = true };

    public CartSynchronizationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateAuthenticatedClient("cart-sync-test-user");
    }

    /// <summary>
    /// Helper method to clear the cart before each test
    /// </summary>
    private async Task ClearCartAsync()
    {
        try
        {
            await _client.DeleteAsync("/api/cart/clear");
        }
        catch
        {
            // Cart might already be empty, ignore errors
        }
    }

    // ---------------------------------------------------------------
    // Test 1: Initial Cart State Sync
    // Verify empty cart is returned on first GET
    // ---------------------------------------------------------------
    [Fact]
    public async Task GetCart_InitialState_ReturnsEmptyCart()
    {
        // Arrange: Clear any existing cart items first
        await ClearCartAsync();

        // Act
        var response = await _client.GetAsync("/api/cart");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        
        // Verify structure
        Assert.True(doc.RootElement.TryGetProperty("id", out var idProp));
        Assert.True(doc.RootElement.TryGetProperty("userId", out var userIdProp));
        Assert.True(doc.RootElement.TryGetProperty("items", out var itemsProp));
        
        // Verify empty
        var itemsArray = itemsProp.EnumerateArray().ToList();
        Assert.Empty(itemsArray);
    }

    // ---------------------------------------------------------------
    // Test 2: Add Item Sync
    // Verify item added to backend is returned in next GET
    // ---------------------------------------------------------------
    [Fact]
    public async Task AddItem_ThenGet_StateIsSynchronized()
    {
        // Arrange
        await ClearCartAsync();
        var addPayload = new { ProductId = 1, Quantity = 2 };

        // Act 1: Add item to cart
        var addResponse = await _client.PostAsJsonAsync("/api/cart", addPayload);
        // Accept both 201 Created and 200 OK responses
        Assert.True(addResponse.StatusCode == System.Net.HttpStatusCode.Created || 
                    addResponse.StatusCode == System.Net.HttpStatusCode.OK,
                    $"Expected 201 or 200, but got {addResponse.StatusCode}");

        // Act 2: Fetch cart from backend
        var getResponse = await _client.GetAsync("/api/cart");
        Assert.Equal(System.Net.HttpStatusCode.OK, getResponse.StatusCode);

        // Assert: Item should be in cart
        var body = await getResponse.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        
        var itemsArray = doc.RootElement.GetProperty("items").EnumerateArray().ToList();
        Assert.Single(itemsArray);
        
        var item = itemsArray[0];
        Assert.Equal(1, item.GetProperty("productId").GetInt32());
        Assert.Equal(2, item.GetProperty("quantity").GetInt32());
        Assert.Equal("Introduction to Information Systems textbook", 
            item.GetProperty("title").GetString());
        Assert.Equal(89.99m, item.GetProperty("price").GetDecimal());
    }

    // ---------------------------------------------------------------
    // Test 3: Update Quantity Sync
    // Verify quantity changes persist in backend
    // ---------------------------------------------------------------
    [Fact]
    public async Task UpdateQuantity_ThenGet_StateIsSynchronized()
    {
        // Arrange: Clear cart and add item first
        await ClearCartAsync();
        var addPayload = new { ProductId = 1, Quantity = 1 };
        var addResponse = await _client.PostAsJsonAsync("/api/cart", addPayload);
        var addBody = await addResponse.Content.ReadAsStringAsync();
        using var addDoc = JsonDocument.Parse(addBody);
        int cartItemId = addDoc.RootElement.GetProperty("id").GetInt32();

        // Act 1: Update quantity
        var updatePayload = new { Quantity = 5 };
        var updateResponse = await _client.PutAsJsonAsync($"/api/cart/{cartItemId}", updatePayload);
        Assert.Equal(System.Net.HttpStatusCode.OK, updateResponse.StatusCode);

        // Act 2: Fetch cart
        var getResponse = await _client.GetAsync("/api/cart");
        Assert.Equal(System.Net.HttpStatusCode.OK, getResponse.StatusCode);

        // Assert: Quantity should be 5
        var body = await getResponse.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        
        var itemsArray = doc.RootElement.GetProperty("items").EnumerateArray().ToList();
        Assert.Single(itemsArray);
        Assert.Equal(5, itemsArray[0].GetProperty("quantity").GetInt32());
    }

    // ---------------------------------------------------------------
    // Test 4: Remove Item Sync
    // Verify removed item no longer in cart
    // ---------------------------------------------------------------
    [Fact]
    public async Task RemoveItem_ThenGet_ItemNotInCart()
    {
        // Arrange: Clear cart and add item first
        await ClearCartAsync();
        var addPayload = new { ProductId = 1, Quantity = 1 };
        var addResponse = await _client.PostAsJsonAsync("/api/cart", addPayload);
        var addBody = await addResponse.Content.ReadAsStringAsync();
        using var addDoc = JsonDocument.Parse(addBody);
        int cartItemId = addDoc.RootElement.GetProperty("id").GetInt32();

        // Act 1: Remove item
        var deleteResponse = await _client.DeleteAsync($"/api/cart/{cartItemId}");
        Assert.Equal(System.Net.HttpStatusCode.OK, deleteResponse.StatusCode);

        // Act 2: Fetch cart
        var getResponse = await _client.GetAsync("/api/cart");
        Assert.Equal(System.Net.HttpStatusCode.OK, getResponse.StatusCode);

        // Assert: Cart should be empty
        var body = await getResponse.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        
        var itemsArray = doc.RootElement.GetProperty("items").EnumerateArray().ToList();
        Assert.Empty(itemsArray);
    }

    // ---------------------------------------------------------------
    // Test 5: Multiple Items Sync
    // Verify multiple items remain synchronized
    // ---------------------------------------------------------------
    [Fact]
    public async Task AddMultipleItems_ThenGet_AllItemsSynchronized()
    {
        // Arrange: Clear cart first
        await ClearCartAsync();

        // Act: Add three items
        var item1 = new { ProductId = 1, Quantity = 1 };
        var item2 = new { ProductId = 2, Quantity = 2 };
        var item3 = new { ProductId = 3, Quantity = 3 };

        await _client.PostAsJsonAsync("/api/cart", item1);
        await _client.PostAsJsonAsync("/api/cart", item2);
        await _client.PostAsJsonAsync("/api/cart", item3);

        // Act: Fetch cart
        var getResponse = await _client.GetAsync("/api/cart");
        Assert.Equal(System.Net.HttpStatusCode.OK, getResponse.StatusCode);

        // Assert: All three items should be present
        var body = await getResponse.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        
        var itemsArray = doc.RootElement.GetProperty("items").EnumerateArray().ToList();
        Assert.Equal(3, itemsArray.Count);

        // Verify quantities
        var quantities = itemsArray.Select(i => i.GetProperty("quantity").GetInt32()).OrderBy(q => q).ToList();
        Assert.Equal(new[] { 1, 2, 3 }, quantities);
    }

    // ---------------------------------------------------------------
    // Test 6: Clear Cart Sync
    // Verify clear operation removes all items from backend
    // ---------------------------------------------------------------
    [Fact]
    public async Task ClearCart_ThenGet_CartIsEmpty()
    {
        // Arrange: Clear cart and add items first
        await ClearCartAsync();
        var item1 = new { ProductId = 1, Quantity = 1 };
        var item2 = new { ProductId = 2, Quantity = 2 };

        await _client.PostAsJsonAsync("/api/cart", item1);
        await _client.PostAsJsonAsync("/api/cart", item2);

        // Act 1: Clear cart
        var clearResponse = await _client.DeleteAsync("/api/cart/clear");
        Assert.Equal(System.Net.HttpStatusCode.OK, clearResponse.StatusCode);

        // Act 2: Fetch cart
        var getResponse = await _client.GetAsync("/api/cart");
        Assert.Equal(System.Net.HttpStatusCode.OK, getResponse.StatusCode);

        // Assert: Cart should be empty
        var body = await getResponse.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        
        var itemsArray = doc.RootElement.GetProperty("items").EnumerateArray().ToList();
        Assert.Empty(itemsArray);
    }

    // ---------------------------------------------------------------
    // Test 7: Increment Existing Item
    // Verify adding same product twice increments quantity
    // ---------------------------------------------------------------
    [Fact]
    public async Task AddSameProduct_Twice_QuantityIncrements()
    {
        // Arrange: Clear cart first
        await ClearCartAsync();
        var addPayload = new { ProductId = 1, Quantity = 2 };

        // Act 1: Add product first time
        await _client.PostAsJsonAsync("/api/cart", addPayload);

        // Act 2: Add same product second time
        await _client.PostAsJsonAsync("/api/cart", addPayload);

        // Act 3: Fetch cart
        var getResponse = await _client.GetAsync("/api/cart");
        Assert.Equal(System.Net.HttpStatusCode.OK, getResponse.StatusCode);

        // Assert: Should have single item with quantity 4
        var body = await getResponse.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        
        var itemsArray = doc.RootElement.GetProperty("items").EnumerateArray().ToList();
        Assert.Single(itemsArray);
        Assert.Equal(4, itemsArray[0].GetProperty("quantity").GetInt32());
    }

    // ---------------------------------------------------------------
    // Test 8: Product Info Persistence
    // Verify product details are stored and returned correctly
    // ---------------------------------------------------------------
    [Fact]
    public async Task AddItem_ProductInfoPersisted()
    {
        // Arrange: Clear cart first
        await ClearCartAsync();

        // Act 1: Add item
        var addPayload = new { ProductId = 1, Quantity = 1 };
        var addResponse = await _client.PostAsJsonAsync("/api/cart", addPayload);
        var addBody = await addResponse.Content.ReadAsStringAsync();

        // Assert 1: Add response includes product info
        using var addDoc = JsonDocument.Parse(addBody);
        Assert.Equal("Introduction to Information Systems textbook", 
            addDoc.RootElement.GetProperty("title").GetString());
        Assert.Equal(89.99m, addDoc.RootElement.GetProperty("price").GetDecimal());
        Assert.Equal("Textbooks", addDoc.RootElement.GetProperty("category").GetString());

        // Act 2: Get cart
        var getResponse = await _client.GetAsync("/api/cart");
        var getBody = await getResponse.Content.ReadAsStringAsync();

        // Assert 2: Get response maintains product info
        using var getDoc = JsonDocument.Parse(getBody);
        var itemsArray = getDoc.RootElement.GetProperty("items").EnumerateArray().ToList();
        var item = itemsArray[0];
        
        Assert.Equal("Introduction to Information Systems textbook", 
            item.GetProperty("title").GetString());
        Assert.Equal(89.99m, item.GetProperty("price").GetDecimal());
        Assert.Equal("Textbooks", item.GetProperty("category").GetString());
        Assert.Equal("John Smith", item.GetProperty("sellerName").GetString());
    }

    // ---------------------------------------------------------------
    // Test 9: Complex Sync Scenario
    // Perform add, update, add, remove, update sequence
    // ---------------------------------------------------------------
    [Fact]
    public async Task ComplexSequence_StateRemainsSynchronized()
    {
        // Arrange: Clear cart first
        await ClearCartAsync();

        // Add item 1
        var response1 = await _client.PostAsJsonAsync("/api/cart", new { ProductId = 1, Quantity = 1 });
        var body1 = await response1.Content.ReadAsStringAsync();
        using var doc1 = JsonDocument.Parse(body1);
        int id1 = doc1.RootElement.GetProperty("id").GetInt32();

        // Add item 2
        await _client.PostAsJsonAsync("/api/cart", new { ProductId = 2, Quantity = 1 });

        // Update item 1 quantity
        await _client.PutAsJsonAsync($"/api/cart/{id1}", new { Quantity = 5 });

        // Add item 3
        var response3 = await _client.PostAsJsonAsync("/api/cart", new { ProductId = 3, Quantity = 2 });
        var body3 = await response3.Content.ReadAsStringAsync();
        using var doc3 = JsonDocument.Parse(body3);
        int id3 = doc3.RootElement.GetProperty("id").GetInt32();

        // Remove item 2
        var getForRemove = await _client.GetAsync("/api/cart");
        var bodyForRemove = await getForRemove.Content.ReadAsStringAsync();
        using var docForRemove = JsonDocument.Parse(bodyForRemove);
        var items = docForRemove.RootElement.GetProperty("items").EnumerateArray().ToList();
        var item2Option = items.FirstOrDefault(i => i.GetProperty("productId").GetInt32() == 2);
        int id2 = item2Option.GetProperty("id").GetInt32();
        
        await _client.DeleteAsync($"/api/cart/{id2}");

        // Get final state
        var finalResponse = await _client.GetAsync("/api/cart");
        var finalBody = await finalResponse.Content.ReadAsStringAsync();
        using var finalDoc = JsonDocument.Parse(finalBody);
        var finalItems = finalDoc.RootElement.GetProperty("items").EnumerateArray().ToList();

        // Assert final state: 2 items
        Assert.Equal(2, finalItems.Count);
        
        // Item 1 should have quantity 5
        var finalItem1Option = finalItems.FirstOrDefault(i => i.GetProperty("productId").GetInt32() == 1);
        Assert.Equal(5, finalItem1Option.GetProperty("quantity").GetInt32());
        
        // Item 3 should have quantity 2
        var finalItem3Option = finalItems.FirstOrDefault(i => i.GetProperty("productId").GetInt32() == 3);
        Assert.Equal(2, finalItem3Option.GetProperty("quantity").GetInt32());
        
        // Item 2 should not exist
        var item2Check = finalItems.FirstOrDefault(i => i.GetProperty("productId").GetInt32() == 2);
        Assert.True(item2Check.ValueKind == JsonValueKind.Undefined, "Item 2 should have been removed");
    }

    // ---------------------------------------------------------------
    // Test 10: Consistency Check - Frontend Calculation
    // Verify that cart can be used to calculate totals correctly
    // ---------------------------------------------------------------
    [Fact]
    public async Task CartTotal_CalculatesCorrectly()
    {
        // Arrange: Clear cart first
        await ClearCartAsync();

        // Add items with known prices
        // Product 1: $89.99 × 2 = $179.98
        // Product 2: $34.99 × 3 = $104.97
        // Total = $284.95

        await _client.PostAsJsonAsync("/api/cart", new { ProductId = 1, Quantity = 2 });
        await _client.PostAsJsonAsync("/api/cart", new { ProductId = 2, Quantity = 3 });

        var getResponse = await _client.GetAsync("/api/cart");
        var body = await getResponse.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        
        var items = doc.RootElement.GetProperty("items").EnumerateArray().ToList();

        // Calculate total (frontend would do this)
        decimal total = 0;
        foreach (var item in items)
        {
            decimal price = item.GetProperty("price").GetDecimal();
            int quantity = item.GetProperty("quantity").GetInt32();
            total += price * quantity;
        }

        // Assert: Total should be $284.95
        Assert.Equal(284.95m, total);
    }
}
