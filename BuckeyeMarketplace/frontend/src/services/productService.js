const API_BASE_URL = `${process.env.REACT_APP_API_URL || 'http://localhost:5107'}/api/products`;

/**
 * Product Service - Handles all product-related API calls
 * Separates API logic from component logic
 */

/**
 * Fetch all products from the backend
 * @returns {Promise<Array>} Array of products
 */
export async function fetchAllProducts() {
  const response = await fetch(API_BASE_URL, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
    },
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
  const response = await fetch(`${API_BASE_URL}/${productId}`, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
    },
  });

  if (!response.ok) {
    throw new Error('Failed to fetch product');
  }

  return await response.json();
}
