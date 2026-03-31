import React from 'react';
import { Link } from 'react-router-dom';
import { useCart } from '../../context/CartContext';
import '../../styles/Header.css';

function Header() {
  const { itemCount } = useCart();

  return (
    <header className="header">
      <div className="header-content">
        <h1 className="header-title">Buckeye Marketplace</h1>
        <Link to="/cart" className="cart-button">
          <span className="cart-icon">🛒</span>
          <span className="cart-label">Cart</span>
          {itemCount > 0 && <span className="cart-badge">{itemCount}</span>}
        </Link>
      </div>
    </header>
  );
}

export default Header;
