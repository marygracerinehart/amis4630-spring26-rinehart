using BuckeyeMarketplaceAPI.Models;
using Xunit;

namespace BuckeyeMarketplaceAPI.Tests;

/// <summary>
/// Pure unit tests for CartToOrderMapper.
/// No database. No HTTP. Just new CartToOrderMapper() and Assert.
/// </summary>
public class CartToOrderMapperTests
{
    private readonly CartToOrderMapper _mapper = new();

    // ───────────────────────────────────────────────────────────────
    // Helper — builds a CartItem with all denormalized fields
    // ───────────────────────────────────────────────────────────────

    private static CartItem MakeCartItem(
        int productId = 1,
        int quantity = 2,
        string title = "Test Product",
        decimal price = 19.99m,
        string imageUrl = "https://example.com/img.jpg",
        string category = "Textbooks",
        string sellerName = "Test Seller") =>
        new()
        {
            ProductId  = productId,
            Quantity   = quantity,
            Title      = title,
            Price      = price,
            ImageUrl   = imageUrl,
            Category   = category,
            SellerName = sellerName
        };

    private static Cart MakeCart(string userId = "user-1", params CartItem[] items) =>
        new()
        {
            UserId = userId,
            Items  = items.ToList()
        };

    // ───────────────────────────────────────────────────────────────
    // MapItem — single CartItem → OrderItem
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public void MapItem_CopiesProductId()
    {
        var cartItem = MakeCartItem(productId: 42);
        var orderItem = _mapper.MapItem(cartItem);
        Assert.Equal(42, orderItem.ProductId);
    }

    [Fact]
    public void MapItem_CopiesQuantity()
    {
        var cartItem = MakeCartItem(quantity: 5);
        var orderItem = _mapper.MapItem(cartItem);
        Assert.Equal(5, orderItem.Quantity);
    }

    [Fact]
    public void MapItem_MapsTitle()
    {
        var cartItem = MakeCartItem(title: "Intro to IS Textbook");
        var orderItem = _mapper.MapItem(cartItem);
        Assert.Equal("Intro to IS Textbook", orderItem.Title);
    }

    [Fact]
    public void MapItem_MapsPriceToUnitPrice()
    {
        var cartItem = MakeCartItem(price: 49.95m);
        var orderItem = _mapper.MapItem(cartItem);
        Assert.Equal(49.95m, orderItem.UnitPrice);
    }

    [Fact]
    public void MapItem_CopiesImageUrl()
    {
        var cartItem = MakeCartItem(imageUrl: "https://example.com/pic.png");
        var orderItem = _mapper.MapItem(cartItem);
        Assert.Equal("https://example.com/pic.png", orderItem.ImageUrl);
    }

    [Fact]
    public void MapItem_CopiesCategory()
    {
        var cartItem = MakeCartItem(category: "Apparel");
        var orderItem = _mapper.MapItem(cartItem);
        Assert.Equal("Apparel", orderItem.Category);
    }

    [Fact]
    public void MapItem_CopiesSellerName()
    {
        var cartItem = MakeCartItem(sellerName: "Jane Doe");
        var orderItem = _mapper.MapItem(cartItem);
        Assert.Equal("Jane Doe", orderItem.SellerName);
    }

    [Fact]
    public void MapItem_NullTitle_DefaultsToUnknown()
    {
        var cartItem = MakeCartItem();
        cartItem.Title = null;
        var orderItem = _mapper.MapItem(cartItem);
        Assert.Equal("Unknown", orderItem.Title);
    }

    [Fact]
    public void MapItem_NullCartItem_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => _mapper.MapItem(null!));
    }

    // ───────────────────────────────────────────────────────────────
    // Map — full Cart → Order
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public void Map_SetsUserIdFromCart()
    {
        var cart = MakeCart("user-abc", MakeCartItem());
        var order = _mapper.Map(cart, "123 Main St");
        Assert.Equal("user-abc", order.UserId);
    }

    [Fact]
    public void Map_SetsStatusToPending()
    {
        var cart = MakeCart("u1", MakeCartItem());
        var order = _mapper.Map(cart, "123 Main St");
        Assert.Equal("Pending", order.Status);
    }

    [Fact]
    public void Map_TrimsShippingAddress()
    {
        var cart = MakeCart("u1", MakeCartItem());
        var order = _mapper.Map(cart, "  456 Oak Ave  ");
        Assert.Equal("456 Oak Ave", order.ShippingAddress);
    }

    [Fact]
    public void Map_SetsOrderDate()
    {
        var before = DateTime.UtcNow;
        var cart = MakeCart("u1", MakeCartItem());
        var order = _mapper.Map(cart, "123 Main St");
        var after = DateTime.UtcNow;

        Assert.InRange(order.OrderDate, before, after);
    }

    [Fact]
    public void Map_SingleItem_CreatesOneOrderItem()
    {
        var cart = MakeCart("u1", MakeCartItem());
        var order = _mapper.Map(cart, "123 Main St");
        Assert.Single(order.Items);
    }

    [Fact]
    public void Map_MultipleItems_CreatesMatchingOrderItems()
    {
        var cart = MakeCart("u1",
            MakeCartItem(productId: 1, quantity: 2, price: 10.00m),
            MakeCartItem(productId: 2, quantity: 1, price: 25.00m),
            MakeCartItem(productId: 3, quantity: 3, price: 5.00m));

        var order = _mapper.Map(cart, "123 Main St");

        Assert.Equal(3, order.Items.Count);
    }

    [Fact]
    public void Map_CalculatesTotalFromItems()
    {
        var cart = MakeCart("u1",
            MakeCartItem(productId: 1, quantity: 2, price: 10.00m),  // 20.00
            MakeCartItem(productId: 2, quantity: 1, price: 25.00m),  // 25.00
            MakeCartItem(productId: 3, quantity: 3, price: 5.00m));  // 15.00

        var order = _mapper.Map(cart, "123 Main St");

        Assert.Equal(60.00m, order.TotalAmount);
    }

    [Fact]
    public void Map_EmptyCart_TotalIsZero()
    {
        var cart = MakeCart("u1"); // no items
        var order = _mapper.Map(cart, "123 Main St");

        Assert.Empty(order.Items);
        Assert.Equal(0m, order.TotalAmount);
    }

    [Fact]
    public void Map_PreservesAllDenormalizedFields()
    {
        var cartItem = MakeCartItem(
            productId: 7,
            quantity: 3,
            title: "OSU Hoodie",
            price: 59.99m,
            imageUrl: "https://img.com/hoodie.jpg",
            category: "Apparel",
            sellerName: "Buckeye Merch");

        var cart = MakeCart("u1", cartItem);
        var order = _mapper.Map(cart, "100 N High St");
        var item = order.Items.First();

        Assert.Equal(7, item.ProductId);
        Assert.Equal(3, item.Quantity);
        Assert.Equal("OSU Hoodie", item.Title);
        Assert.Equal(59.99m, item.UnitPrice);
        Assert.Equal("https://img.com/hoodie.jpg", item.ImageUrl);
        Assert.Equal("Apparel", item.Category);
        Assert.Equal("Buckeye Merch", item.SellerName);
    }

    // ───────────────────────────────────────────────────────────────
    // Error cases
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public void Map_NullCart_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => _mapper.Map(null!, "123 Main St"));
    }

    [Fact]
    public void Map_NullShippingAddress_Throws()
    {
        var cart = MakeCart("u1", MakeCartItem());
        Assert.Throws<ArgumentException>(() => _mapper.Map(cart, null!));
    }

    [Fact]
    public void Map_EmptyShippingAddress_Throws()
    {
        var cart = MakeCart("u1", MakeCartItem());
        Assert.Throws<ArgumentException>(() => _mapper.Map(cart, ""));
    }

    [Fact]
    public void Map_WhitespaceShippingAddress_Throws()
    {
        var cart = MakeCart("u1", MakeCartItem());
        Assert.Throws<ArgumentException>(() => _mapper.Map(cart, "   "));
    }
}
