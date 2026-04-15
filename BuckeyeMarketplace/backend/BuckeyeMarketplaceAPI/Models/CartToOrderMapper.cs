namespace BuckeyeMarketplaceAPI.Models;

/// <summary>
/// Pure-logic mapper that converts a Cart + shipping address into an Order.
/// No database, no HTTP, no I/O — just plain object-to-object mapping.
/// </summary>
public class CartToOrderMapper
{
    /// <summary>
    /// Maps a Cart and its CartItems into a new Order with OrderItems.
    /// Each CartItem's denormalized fields (Title, Price, ImageUrl, etc.)
    /// are carried over so the order is a snapshot of the cart at checkout time.
    /// </summary>
    public Order Map(Cart cart, string shippingAddress)
    {
        ArgumentNullException.ThrowIfNull(cart);

        if (string.IsNullOrWhiteSpace(shippingAddress))
            throw new ArgumentException("Shipping address is required.", nameof(shippingAddress));

        var order = new Order
        {
            UserId = cart.UserId,
            OrderDate = DateTime.UtcNow,
            Status = "Pending",
            ShippingAddress = shippingAddress.Trim()
        };

        foreach (var cartItem in cart.Items)
        {
            order.Items.Add(MapItem(cartItem));
        }

        order.TotalAmount = order.CalculateTotal();

        return order;
    }

    /// <summary>
    /// Maps a single CartItem to an OrderItem, carrying over all
    /// denormalized product fields.
    /// </summary>
    public OrderItem MapItem(CartItem cartItem)
    {
        ArgumentNullException.ThrowIfNull(cartItem);

        return new OrderItem
        {
            ProductId  = cartItem.ProductId,
            Quantity   = cartItem.Quantity,
            Title      = cartItem.Title ?? "Unknown",
            UnitPrice  = cartItem.Price,
            ImageUrl   = cartItem.ImageUrl,
            Category   = cartItem.Category,
            SellerName = cartItem.SellerName
        };
    }
}
