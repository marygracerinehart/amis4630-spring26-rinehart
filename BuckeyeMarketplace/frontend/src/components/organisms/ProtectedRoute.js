import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';

/**
 * ProtectedRoute - Wraps routes that require authentication
 * Redirects unauthenticated users to /login, preserving the intended destination
 * so they can be sent back after logging in.
 *
 * @param {ReactNode} children - The protected page component
 * @param {string[]} [roles] - Optional array of required roles (e.g., ['Admin'])
 */
function ProtectedRoute({ children, roles }) {
  const { isAuthenticated, user, loading } = useAuth();
  const location = useLocation();

  // Wait for auth state to be restored from localStorage
  if (loading) {
    return (
      <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '50vh' }}>
        <p>Loading...</p>
      </div>
    );
  }

  // Not logged in → redirect to login, saving where they wanted to go
  if (!isAuthenticated) {
    return <Navigate to="/login" state={{ from: location.pathname }} replace />;
  }

  // Logged in but missing a required role → redirect to home
  if (roles && roles.length > 0) {
    const userRoles = user?.roles || [];
    const hasRequiredRole = roles.some((role) => userRoles.includes(role));
    if (!hasRequiredRole) {
      return <Navigate to="/" replace />;
    }
  }

  return children;
}

export default ProtectedRoute;
