import { apiFetch } from './apiClient';

const API_PATH = '/api/orders';

/**
 * Order Service - Handles all order-related API calls
 * Uses apiFetch for automatic JWT token injection and 401 refresh-retry
 */

/**
 * Place an order from the current user's cart.
 * The backend converts all cart items into order items, reduces stock, and clears the cart.
 * @param {string} shippingAddress - The shipping address for the order
 * @returns {Promise<Object>} The created order object with items
 */
export async function placeOrder(shippingAddress) {
  const response = await apiFetch(API_PATH, {
    method: 'POST',
    body: JSON.stringify({ shippingAddress }),
  });

  if (!response.ok) {
    const errorData = await response.json().catch(() => null);
    throw new Error(errorData?.message || 'Failed to place order.');
  }

  return await response.json();
}

/**
 * Fetch current user's order history, newest first.
 * User ID is extracted from the JWT on the server — not passed in the URL.
 * @returns {Promise<Array>} Array of order objects with items
 */
export async function getOrders() {
  const response = await apiFetch(`${API_PATH}/mine`, {
    method: 'GET',
  });

  if (!response.ok) {
    throw new Error('Failed to fetch orders.');
  }

  return await response.json();
}

/**
 * Fetch a single order by ID.
 * @param {number} orderId - The order ID
 * @returns {Promise<Object>} The order object with items
 */
export async function getOrderById(orderId) {
  const response = await apiFetch(`${API_PATH}/${orderId}`, {
    method: 'GET',
  });

  if (!response.ok) {
    const errorData = await response.json().catch(() => null);
    throw new Error(errorData?.message || 'Failed to fetch order.');
  }

  return await response.json();
}
