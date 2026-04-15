import React, { createContext, useContext, useState, useCallback, useEffect } from 'react';
import * as authService from '../services/authService';

const AuthContext = createContext();

/**
 * AuthProvider - Manages authentication state (user, token, login/logout)
 * Wraps the app to provide auth context to all components
 */
export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [token, setToken] = useState(null);
  const [loading, setLoading] = useState(true);

  // Restore auth state from localStorage on mount
  useEffect(() => {
    const stored = authService.getStoredAuth();
    if (stored) {
      setToken(stored.token);
      setUser(stored.user);
    }
    setLoading(false);
  }, []);

  /**
   * Log in a user with email and password
   * @param {string} email
   * @param {string} password
   * @returns {Promise<Object>} The authenticated user object
   */
  const login = useCallback(async (email, password) => {
    const data = await authService.login(email, password);
    authService.saveAuthData(data);
    const userData = {
      userId: data.userId,
      email: data.email,
      fullName: data.fullName,
      roles: data.roles,
    };
    setToken(data.token);
    setUser(userData);
    return userData;
  }, []);

  /**
   * Register a new user
   * @param {string} email
   * @param {string} password
   * @param {string} confirmPassword
   * @param {string} fullName
   * @returns {Promise<Object>} The authenticated user object
   */
  const register = useCallback(async (email, password, confirmPassword, fullName) => {
    const data = await authService.register(email, password, confirmPassword, fullName);
    authService.saveAuthData(data);
    const userData = {
      userId: data.userId,
      email: data.email,
      fullName: data.fullName,
      roles: data.roles,
    };
    setToken(data.token);
    setUser(userData);
    return userData;
  }, []);

  /**
   * Log out the current user
   * - Revokes the refresh token on the server (best-effort)
   * - Clears localStorage
   * - Resets React state
   */
  const logout = useCallback(async () => {
    await authService.revokeToken();
    authService.clearAuthData();
    setToken(null);
    setUser(null);
  }, []);

  const isAuthenticated = !!token;
  const isAdmin = user?.roles?.includes('Admin') || false;

  return (
    <AuthContext.Provider
      value={{
        user,
        token,
        loading,
        isAuthenticated,
        isAdmin,
        login,
        register,
        logout,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

/**
 * Custom hook to access auth context
 * @returns {Object} Auth state and actions
 */
export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}
