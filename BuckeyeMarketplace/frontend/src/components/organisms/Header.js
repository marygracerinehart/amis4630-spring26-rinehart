import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useCart, CART_ACTIONS } from '../../context/CartContext';
import { useAuth } from '../../context/AuthContext';
import { useNotification } from '../../context/NotificationContext';
import '../../styles/Header.css';

function Header() {
  const { itemCount, dispatch } = useCart();
  const { isAuthenticated, user, logout } = useAuth();
  const { addNotification } = useNotification();
  const navigate = useNavigate();

  const handleLogout = async () => {
    const firstName = user?.fullName?.split(' ')[0] || 'User';
    await logout();
    // Clear cart locally (no API call — token is already revoked)
    dispatch({ type: CART_ACTIONS.SET_CART, payload: [] });
    addNotification(`Goodbye, ${firstName}! You've been logged out.`, 'info');
    navigate('/login');
  };

  return (
    <header className="header">
      <div className="header-content">
        <Link to="/" className="header-title-link">
          <h1 className="header-title">Buckeye Marketplace</h1>
        </Link>
        <div className="header-actions">
          {isAuthenticated ? (
            <>
              <span className="header-greeting">Hi, {user?.fullName?.split(' ')[0]}</span>
              <button className="header-auth-button header-logout" onClick={handleLogout}>
                Logout
              </button>
            </>
          ) : (
            <>
              <Link to="/login" className="header-auth-button header-login">
                Login
              </Link>
              <Link to="/register" className="header-auth-button header-register">
                Register
              </Link>
            </>
          )}
          <Link to="/cart" className="cart-button">
            <span className="cart-icon">🛒</span>
            <span className="cart-label">Cart</span>
            {itemCount > 0 && <span className="cart-badge">{itemCount}</span>}
          </Link>
        </div>
      </div>
    </header>
  );
}

export default Header;
