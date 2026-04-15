using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BuckeyeMarketplaceAPI.Data;
using BuckeyeMarketplaceAPI.Models;
using System.Linq;

namespace BuckeyeMarketplaceAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Extracts the authenticated user's ID from the JWT claims.
        /// </summary>
        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("User ID not found in token.");
        }

        /// <summary>
        /// POST /api/order — Place an order from the current user's cart.
        /// Converts all cart items into order items, reduces stock, and clears the cart.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Order>> PlaceOrder([FromBody] PlaceOrderDto dto)
        {
            var userId = GetUserId();

            if (string.IsNullOrWhiteSpace(dto.ShippingAddress))
            {
                return BadRequest(new { message = "Shipping address is required." });
            }

            // Load the user's cart with items
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.Items.Any())
            {
                return BadRequest(new { message = "Your cart is empty. Add items before placing an order." });
            }

            // Validate stock availability for every item
            foreach (var cartItem in cart.Items)
            {
                var product = await _context.Products.FindAsync(cartItem.ProductId);
                if (product == null)
                {
                    return BadRequest(new { message = $"Product '{cartItem.Title}' (ID {cartItem.ProductId}) no longer exists." });
                }
                if (product.StockQuantity < cartItem.Quantity)
                {
                    return BadRequest(new { message = $"Insufficient stock for '{product.Title}'. Requested: {cartItem.Quantity}, Available: {product.StockQuantity}." });
                }
            }

            // Build the order
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                TotalAmount = 0,
                ShippingAddress = dto.ShippingAddress.Trim()
            };

            foreach (var cartItem in cart.Items)
            {
                var product = await _context.Products.FindAsync(cartItem.ProductId);

                var orderItem = new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    Title = cartItem.Title ?? product!.Title ?? "Unknown",
                    UnitPrice = cartItem.Price,
                    ImageUrl = cartItem.ImageUrl,
                    Category = cartItem.Category,
                    SellerName = cartItem.SellerName
                };

                order.Items.Add(orderItem);
                order.TotalAmount += orderItem.UnitPrice * orderItem.Quantity;

                // Reduce stock
                product!.StockQuantity -= cartItem.Quantity;
            }

            _context.Orders.Add(order);

            // Clear the cart
            _context.CartItems.RemoveRange(cart.Items);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        /// <summary>
        /// GET /api/orders/mine — Current user's order history.
        /// User ID comes from the JWT, not the URL.
        /// </summary>
        [HttpGet("mine")]
        public async Task<ActionResult<IEnumerable<Order>>> GetMyOrders()
        {
            var userId = GetUserId();

            var orders = await _context.Orders
                .Include(o => o.Items)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return Ok(orders);
        }

        /// <summary>
        /// GET /api/order/{id} — Get a specific order by ID (must belong to the current user).
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var userId = GetUserId();

            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
            {
                return NotFound(new { message = $"Order with ID {id} not found." });
            }

            return Ok(order);
        }

        /// <summary>
        /// PUT /api/orders/{orderId}/status — Update an order's status (admin only).
        /// Valid statuses: Pending, Processing, Shipped, Delivered, Cancelled.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var validStatuses = new[] { "Pending", "Processing", "Shipped", "Delivered", "Cancelled" };
            if (!validStatuses.Contains(model.Status))
                return BadRequest(new { message = $"Invalid status '{model.Status}'. Must be one of: {string.Join(", ", validStatuses)}." });

            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return NotFound(new { message = $"Order with ID {orderId} not found." });

            order.Status = model.Status;
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Order {orderId} status updated to '{model.Status}'." });
        }
    }

    /// <summary>
    /// DTO for placing an order — carries the shipping address from the client.
    /// </summary>
    public class PlaceOrderDto
    {
        public string ShippingAddress { get; set; } = string.Empty;
    }
}
