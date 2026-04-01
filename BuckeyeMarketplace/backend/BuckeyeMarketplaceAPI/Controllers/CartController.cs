using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BuckeyeMarketplaceAPI.Data;
using BuckeyeMarketplaceAPI.Models;

namespace BuckeyeMarketplaceAPI.Controllers
{
    public class AddToCartRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }

    public class UpdateCartItemRequest
    {
        public int Quantity { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;

        // Hardcoded user ID — will be replaced with auth in M5
        private const string HardcodedUserId = "user-1";

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get or create a cart for the given user (persisted to the database).
        /// </summary>
        private async Task<Cart> GetOrCreateCartAsync(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        /// <summary>
        /// GET /api/cart — Retrieve cart contents for the current user
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<Cart>> GetCart()
        {
            var cart = await GetOrCreateCartAsync(HardcodedUserId);
            return Ok(cart);
        }

        /// <summary>
        /// POST /api/cart — Add an item to the cart
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CartItem>> AddToCart([FromBody] AddToCartRequest request)
        {
            if (request.Quantity <= 0)
            {
                return BadRequest(new { message = "Quantity must be greater than zero." });
            }

            // Look up the product from the database
            var product = await _context.Products.FindAsync(request.ProductId);
            if (product == null)
            {
                return NotFound(new { message = $"Product with ID {request.ProductId} not found." });
            }

            // Check if product is out of stock
            if (product.StockQuantity <= 0)
            {
                return BadRequest(new { message = $"Sorry, {product.Title} is out of stock." });
            }

            // Check if requested quantity exceeds available stock
            if (request.Quantity > product.StockQuantity)
            {
                return BadRequest(new { message = $"Cannot add {request.Quantity} units. Only {product.StockQuantity} available in stock." });
            }

            var cart = await GetOrCreateCartAsync(HardcodedUserId);

            // Check if item already exists in cart
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += request.Quantity;
                await _context.SaveChangesAsync();
                return Ok(existingItem);
            }

            var cartItem = new CartItem
            {
                CartId = cart.Id,
                ProductId = product.Id,
                Quantity = request.Quantity,
                Title = product.Title,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                Category = product.Category,
                SellerName = product.SellerName
            };

            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCart), null, cartItem);
        }

        /// <summary>
        /// PUT /api/cart/{cartItemId} — Update item quantity
        /// </summary>
        [HttpPut("{cartItemId}")]
        public async Task<ActionResult<CartItem>> UpdateCartItem(int cartItemId, [FromBody] UpdateCartItemRequest request)
        {
            if (request.Quantity <= 0)
            {
                return BadRequest(new { message = "Quantity must be greater than zero." });
            }

            var item = await _context.CartItems.FindAsync(cartItemId);

            if (item == null)
            {
                return NotFound(new { message = $"Cart item with ID {cartItemId} not found." });
            }

            // Check if quantity exceeds stock
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product != null)
            {
                if (product.StockQuantity <= 0)
                {
                    return BadRequest(new { message = $"Sorry, {product.Title} is out of stock." });
                }

                if (request.Quantity > product.StockQuantity)
                {
                    return BadRequest(new { message = $"Cannot update to {request.Quantity} units. Only {product.StockQuantity} available in stock." });
                }
            }

            item.Quantity = request.Quantity;
            await _context.SaveChangesAsync();
            return Ok(item);
        }

        /// <summary>
        /// DELETE /api/cart/{cartItemId} — Remove an item from the cart
        /// </summary>
        [HttpDelete("{cartItemId}")]
        public async Task<IActionResult> RemoveCartItem(int cartItemId)
        {
            var item = await _context.CartItems.FindAsync(cartItemId);

            if (item == null)
            {
                return NotFound(new { message = $"Cart item with ID {cartItemId} not found." });
            }

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Item removed from cart." });
        }

        /// <summary>
        /// DELETE /api/cart/clear — Clear the entire cart
        /// </summary>
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var cart = await GetOrCreateCartAsync(HardcodedUserId);
            _context.CartItems.RemoveRange(cart.Items);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Cart cleared." });
        }
    }
}
