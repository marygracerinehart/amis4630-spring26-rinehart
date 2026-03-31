using Microsoft.AspNetCore.Mvc;
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
        // Hardcoded user ID — will be replaced with auth in M5
        private const string HardcodedUserId = "user-1";

        // In-memory cart storage shared across requests
        private static readonly List<Cart> _carts = new();
        private static int _nextCartItemId = 1;

        // Reference to products — shares the same in-memory list as ProductsController
        private static List<Product>? _products;

        private static List<Product> Products
        {
            get
            {
                if (_products == null)
                {
                    // Initialize with the same seed data structure; in a real app this would be a DB
                    _products = new List<Product>();
                }
                return _products;
            }
        }

        private Cart GetOrCreateCart(string userId)
        {
            var cart = _carts.FirstOrDefault(c => c.UserId == userId);
            if (cart == null)
            {
                cart = new Cart
                {
                    Id = _carts.Count + 1,
                    UserId = userId
                };
                _carts.Add(cart);
            }
            return cart;
        }

        /// <summary>
        /// GET /api/cart — Retrieve cart contents for the current user
        /// </summary>
        [HttpGet]
        public ActionResult<Cart> GetCart()
        {
            var cart = GetOrCreateCart(HardcodedUserId);
            return Ok(cart);
        }

        /// <summary>
        /// POST /api/cart — Add an item to the cart
        /// </summary>
        [HttpPost]
        public ActionResult<CartItem> AddToCart([FromBody] AddToCartRequest request)
        {
            if (request.Quantity <= 0)
            {
                return BadRequest(new { message = "Quantity must be greater than zero." });
            }

            // Look up the product from the ProductsController's shared list
            var product = ProductsController.GetProductById(request.ProductId);
            if (product == null)
            {
                return NotFound(new { message = $"Product with ID {request.ProductId} not found." });
            }

            var cart = GetOrCreateCart(HardcodedUserId);

            // Check if item already exists in cart
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += request.Quantity;
                return Ok(existingItem);
            }

            var cartItem = new CartItem
            {
                Id = _nextCartItemId++,
                ProductId = product.Id,
                Quantity = request.Quantity,
                Title = product.Title,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                Category = product.Category,
                SellerName = product.SellerName
            };

            cart.Items.Add(cartItem);
            return CreatedAtAction(nameof(GetCart), null, cartItem);
        }

        /// <summary>
        /// PUT /api/cart/{cartItemId} — Update item quantity
        /// </summary>
        [HttpPut("{cartItemId}")]
        public ActionResult<CartItem> UpdateCartItem(int cartItemId, [FromBody] UpdateCartItemRequest request)
        {
            if (request.Quantity <= 0)
            {
                return BadRequest(new { message = "Quantity must be greater than zero." });
            }

            var cart = GetOrCreateCart(HardcodedUserId);
            var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);

            if (item == null)
            {
                return NotFound(new { message = $"Cart item with ID {cartItemId} not found." });
            }

            item.Quantity = request.Quantity;
            return Ok(item);
        }

        /// <summary>
        /// DELETE /api/cart/{cartItemId} — Remove an item from the cart
        /// </summary>
        [HttpDelete("{cartItemId}")]
        public IActionResult RemoveCartItem(int cartItemId)
        {
            var cart = GetOrCreateCart(HardcodedUserId);
            var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);

            if (item == null)
            {
                return NotFound(new { message = $"Cart item with ID {cartItemId} not found." });
            }

            cart.Items.Remove(item);
            return Ok(new { message = "Item removed from cart." });
        }

        /// <summary>
        /// DELETE /api/cart/clear — Clear the entire cart
        /// </summary>
        [HttpDelete("clear")]
        public IActionResult ClearCart()
        {
            var cart = GetOrCreateCart(HardcodedUserId);
            cart.Items.Clear();
            return Ok(new { message = "Cart cleared." });
        }
    }
}
