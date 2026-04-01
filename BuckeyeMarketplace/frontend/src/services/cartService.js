const API_BASE_URL = `${process.env.REACT_APP_API_URL || 'http://localhost:5107'}/api/cart`;

/**
 * Cart Service - Handles all cart-related API calls
 * Separates API logic from state management
 */

/**
 * Fetch the current cart from the backend
 * @returns {Promise<Array>} Array of cart items
 */
export async function fetchCart() {
  const response = await fetch(API_BASE_URL, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
    },
  });

  if (!response.ok) {
    throw new Error('Failed to fetch cart');
  }

  const cartData = await response.json();
  return cartData.items || [];
}

/**
 * Add an item to the cart
 * @param {number} productId - Product ID to add
 * @param {number} quantity - Quantity to add (default: 1)
 * @returns {Promise<Array>} Updated cart items
 */
export async function addItemToCart(productId, quantity = 1) {
  const response = await fetch(API_BASE_URL, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({
      productId,
      quantity,
    }),
  });

  if (!response.ok) {
    throw new Error('Failed to add item to cart');
  }

  // Refresh cart from server
  return fetchCart();
}

/**
 * Remove an item from the cart
 * @param {number} cartItemId - Cart item ID to remove
 * @returns {Promise<Array>} Updated cart items
 */
export async function removeItemFromCart(cartItemId) {
  const response = await fetch(`${API_BASE_URL}/${cartItemId}`, {
    method: 'DELETE',
    headers: {
      'Content-Type': 'application/json',
    },
  });

  if (!response.ok) {
    throw new Error('Failed to remove item from cart');
  }

  // Refresh cart from server
  return fetchCart();
}

/**
 * Update the quantity of an item in the cart
 * @param {number} cartItemId - Cart item ID to update
 * @param {number} quantity - New quantity
 * @returns {Promise<Array>} Updated cart items
 */
export async function updateCartItemQuantity(cartItemId, quantity) {
  const response = await fetch(`${API_BASE_URL}/${cartItemId}`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ quantity: Math.max(1, quantity) }),
  });

  if (!response.ok) {
    throw new Error('Failed to update item quantity');
  }

  // Refresh cart from server
  return fetchCart();
}

/**
 * Clear all items from the cart
 * @returns {Promise<Array>} Empty cart items array
 */
export async function clearEntireCart() {
  const response = await fetch(`${API_BASE_URL}/clear`, {
    method: 'DELETE',
    headers: {
      'Content-Type': 'application/json',
    },
  });

  if (!response.ok) {
    throw new Error('Failed to clear cart');
  }

  return [];
}
