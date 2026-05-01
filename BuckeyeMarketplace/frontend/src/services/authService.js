const API_BASE_URL = `${
  process.env.REACT_APP_API_URL?.trim() ||
  (typeof window !== 'undefined' && window.location.hostname === 'localhost'
    ? 'http://localhost:5107'
    : '')
}/api/auth`;

/**
 * Auth Service - Handles all authentication-related API calls
 * Separates API logic from state management
 */

/**
 * Log in with email and password
 * @param {string} email - User email
 * @param {string} password - User password
 * @returns {Promise<Object>} Auth response with token, user info, and roles
 */
export async function login(email, password) {
  const response = await fetch(`${API_BASE_URL}/login`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ email, password }),
  });

  if (!response.ok) {
    const errorData = await response.json().catch(() => null);
    throw new Error(errorData?.message || 'Invalid email or password.');
  }

  return await response.json();
}

/**
 * Register a new user account
 * @param {string} email - User email
 * @param {string} password - User password
 * @param {string} confirmPassword - Password confirmation
 * @param {string} fullName - User full name
 * @returns {Promise<Object>} Auth response with token, user info, and roles
 */
export async function register(email, password, confirmPassword, fullName) {
  const response = await fetch(`${API_BASE_URL}/register`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ email, password, confirmPassword, fullName }),
  });

  if (!response.ok) {
    const errorData = await response.json().catch(() => null);
    const message =
      errorData?.message ||
      (errorData?.errors && Object.values(errorData.errors).flat().join(' ')) ||
      'Registration failed.';
    throw new Error(message);
  }

  return await response.json();
}

/**
 * Refresh an expired access token using a refresh token
 * @param {string} token - Expired access token
 * @param {string} refreshToken - Valid refresh token
 * @returns {Promise<Object>} New auth response
 */
export async function refreshToken(token, refreshToken) {
  const response = await fetch(`${API_BASE_URL}/refresh`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ token, refreshToken }),
  });

  if (!response.ok) {
    throw new Error('Failed to refresh token.');
  }

  return await response.json();
}

/**
 * Store auth data in localStorage
 * @param {Object} authData - Auth response from the API
 */
export function saveAuthData(authData) {
  localStorage.setItem('token', authData.token);
  localStorage.setItem('refreshToken', authData.refreshToken);
  localStorage.setItem('user', JSON.stringify({
    userId: authData.userId,
    email: authData.email,
    fullName: authData.fullName,
    roles: authData.roles,
  }));
}

/**
 * Revoke the refresh token on the server (server-side logout).
 * Best-effort: errors are silenced so logout always completes client-side.
 * @returns {Promise<void>}
 */
export async function revokeToken() {
  const refreshTokenValue = localStorage.getItem('refreshToken');
  if (!refreshTokenValue) return;

  try {
    await fetch(`${API_BASE_URL}/revoke`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ refreshToken: refreshTokenValue }),
    });
  } catch {
    // Silently ignore — client will clear local data regardless
  }
}

/**
 * Get stored auth data from localStorage
 * @returns {Object|null} Stored user and tokens, or null
 */
export function getStoredAuth() {
  const token = localStorage.getItem('token');
  const user = localStorage.getItem('user');

  if (!token || !user) return null;

  return {
    token,
    refreshToken: localStorage.getItem('refreshToken'),
    user: JSON.parse(user),
  };
}

/**
 * Clear auth data from localStorage
 */
export function clearAuthData() {
  localStorage.removeItem('token');
  localStorage.removeItem('refreshToken');
  localStorage.removeItem('user');
}
