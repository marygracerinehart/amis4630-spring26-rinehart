import { apiFetch } from './apiClient';

const API_PATH = '/api/admin';
const PRODUCTS_API_PATH = '/api/products';

/**
 * Admin Service — Handles all admin-only API calls.
 * Uses apiFetch for automatic JWT token injection and 401 refresh-retry.
 * All endpoints require the "Admin" role.
 */

// ════════════════════════════════════════════════
//  DASHBOARD / STATS
// ════════════════════════════════════════════════

/**
 * Fetch high-level marketplace statistics.
 * @returns {Promise<Object>} { totalUsers, totalProducts, totalOrders, totalRevenue, pendingOrders }
 */
export async function getDashboardStats() {
  const response = await apiFetch(`${API_PATH}/dashboard`, { method: 'GET' });

  if (!response.ok) {
    const errorData = await response.json().catch(() => null);
    throw new Error(errorData?.message || 'Failed to fetch dashboard stats.');
  }

  return await response.json();
}

// ════════════════════════════════════════════════
//  USER MANAGEMENT
// ════════════════════════════════════════════════

/**
 * Fetch all registered users with their roles.
 * @returns {Promise<Array>} Array of user objects
 */
export async function getUsers() {
  const response = await apiFetch(`${API_PATH}/users`, { method: 'GET' });

  if (!response.ok) {
    throw new Error('Failed to fetch users.');
  }

  return await response.json();
}

/**
 * Delete a user account.
 * @param {string} userId - The user ID to delete
 * @returns {Promise<Object>} Success message
 */
export async function deleteUser(userId) {
  const response = await apiFetch(`${API_PATH}/users/${userId}`, { method: 'DELETE' });

  if (!response.ok) {
    const errorData = await response.json().catch(() => null);
    throw new Error(errorData?.message || 'Failed to delete user.');
  }

  return await response.json();
}

// ════════════════════════════════════════════════
//  ORDER MANAGEMENT
// ════════════════════════════════════════════════

/**
 * Fetch all orders across all users.
 * @returns {Promise<Array>} Array of order objects with items
 */
export async function getAllOrders() {
  const response = await apiFetch(`${API_PATH}/orders`, { method: 'GET' });

  if (!response.ok) {
    throw new Error('Failed to fetch orders.');
  }

  return await response.json();
}

/**
 * Update an order's status.
 * @param {number} orderId - The order ID
 * @param {string} status - New status (e.g. "Shipped", "Delivered", "Cancelled")
 * @returns {Promise<Object>} Success message
 */
export async function updateOrderStatus(orderId, status) {
  const response = await apiFetch(`/api/orders/${orderId}/status`, {
    method: 'PUT',
    body: JSON.stringify({ status }),
  });

  if (!response.ok) {
    const errorData = await response.json().catch(() => null);
    throw new Error(errorData?.message || 'Failed to update order status.');
  }

  return await response.json();
}

// ════════════════════════════════════════════════
//  PRODUCT MANAGEMENT
// ════════════════════════════════════════════════

/**
 * Fetch all products.
 * @returns {Promise<Array>} Array of product objects
 */
export async function getProducts() {
  const response = await apiFetch(`${PRODUCTS_API_PATH}`, { method: 'GET' });

  if (!response.ok) {
    throw new Error('Failed to fetch products.');
  }

  return await response.json();
}

/**
 * Create a new product (Admin only).
 * @param {Object} product - Product data
 * @returns {Promise<Object>} Created product
 */
export async function createProduct(product) {
  const response = await apiFetch(`${PRODUCTS_API_PATH}`, {
    method: 'POST',
    body: JSON.stringify(product),
  });

  if (!response.ok) {
    const errorData = await response.json().catch(() => null);
    throw new Error(errorData?.message || 'Failed to create product.');
  }

  return await response.json();
}

/**
 * Update an existing product (Admin only).
 * @param {number} productId - The product ID
 * @param {Object} product - Updated product data
 * @returns {Promise<void>}
 */
export async function updateProduct(productId, product) {
  const response = await apiFetch(`${PRODUCTS_API_PATH}/${productId}`, {
    method: 'PUT',
    body: JSON.stringify(product),
  });

  if (!response.ok) {
    const errorData = await response.json().catch(() => null);
    throw new Error(errorData?.message || 'Failed to update product.');
  }
}

/**
 * Delete a product (Admin only).
 * @param {number} productId - The product ID to delete
 * @returns {Promise<void>}
 */
export async function deleteProduct(productId) {
  const response = await apiFetch(`${PRODUCTS_API_PATH}/${productId}`, {
    method: 'DELETE',
  });

  if (!response.ok) {
    const errorData = await response.json().catch(() => null);
    throw new Error(errorData?.message || 'Failed to delete product.');
  }
}
