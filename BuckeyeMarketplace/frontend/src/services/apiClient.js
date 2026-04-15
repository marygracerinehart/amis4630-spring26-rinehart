import { getStoredAuth, saveAuthData, clearAuthData } from './authService';

const BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5107';

/**
 * API Client - Centralized fetch wrapper with automatic token injection
 *
 * Features:
 *  1. Reads JWT from localStorage and attaches Authorization: Bearer header
 *  2. On 401 response, attempts a silent token refresh using the refresh token
 *  3. If refresh succeeds, retries the original request with the new token
 *  4. If refresh fails, clears auth data (forces logout on next render)
 *
 * Usage:  import { apiFetch } from './apiClient';
 *         const data = await apiFetch('/api/cart');
 */

let isRefreshing = false;
let refreshPromise = null;

/**
 * Attempt to refresh the access token using the stored refresh token.
 * Deduplicates concurrent refresh attempts so only one request fires.
 * @returns {Promise<string|null>} New access token, or null on failure
 */
async function refreshAccessToken() {
  // If a refresh is already in flight, wait for it
  if (isRefreshing) {
    return refreshPromise;
  }

  isRefreshing = true;
  refreshPromise = (async () => {
    try {
      const stored = getStoredAuth();
      if (!stored?.token || !stored?.refreshToken) {
        return null;
      }

      const response = await fetch(`${BASE_URL}/api/auth/refresh`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          token: stored.token,
          refreshToken: stored.refreshToken,
        }),
      });

      if (!response.ok) {
        return null;
      }

      const data = await response.json();
      saveAuthData(data);
      return data.token;
    } catch {
      return null;
    } finally {
      isRefreshing = false;
      refreshPromise = null;
    }
  })();

  return refreshPromise;
}

/**
 * Authenticated fetch wrapper.
 * Automatically attaches the JWT token and handles 401 refresh-retry.
 *
 * @param {string} url - Absolute URL or path starting with /api/...
 * @param {RequestInit} [options={}] - Standard fetch options (method, body, etc.)
 * @returns {Promise<Response>} The fetch Response object
 */
export async function apiFetch(url, options = {}) {
  // Resolve relative paths against the base URL
  const fullUrl = url.startsWith('http') ? url : `${BASE_URL}${url}`;

  // Build headers with token injection
  const stored = getStoredAuth();
  const headers = {
    'Content-Type': 'application/json',
    ...options.headers,
  };

  if (stored?.token) {
    headers['Authorization'] = `Bearer ${stored.token}`;
  }

  // First attempt
  let response = await fetch(fullUrl, { ...options, headers });

  // If 401, try refreshing the token and retry once
  if (response.status === 401 && stored?.refreshToken) {
    const newToken = await refreshAccessToken();

    if (newToken) {
      headers['Authorization'] = `Bearer ${newToken}`;
      response = await fetch(fullUrl, { ...options, headers });
    } else {
      // Refresh failed — clear auth so the UI reflects logged-out state
      clearAuthData();
    }
  }

  return response;
}

export default apiFetch;
