import { apiFetch } from './apiClient';

const API_PATH = '/api/products';

/**
 * Product Service - Handles all product-related API calls
 * Uses apiFetch for automatic JWT token injection and 401 refresh-retry
 */

/**
 * Fetch all products from the backend
 * @returns {Promise<Array>} Array of products
 */
export async function fetchAllProducts() {
  const response = await apiFetch(API_PATH, {
    method: 'GET',
  });

  if (!response.ok) {
    throw new Error('Failed to fetch products');
  }

  return await response.json();
}

/**
 * Fetch a single product by ID
 * @param {number} productId - Product ID
 * @returns {Promise<Object>} Product object
 */
export async function fetchProductById(productId) {
  const response = await apiFetch(`${API_PATH}/${productId}`, {
    method: 'GET',
  });

  if (!response.ok) {
    throw new Error('Failed to fetch product');
  }

  return await response.json();
}
