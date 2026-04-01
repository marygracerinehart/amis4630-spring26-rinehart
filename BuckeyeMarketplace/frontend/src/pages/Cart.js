import React from 'react';
import { Link } from 'react-router-dom';
import { useCart } from '../context/CartContext';
import CartPage from '../components/organisms/CartPage';
import '../styles/Cart.css';

function Cart() {
  const { items, itemCount, subtotal, total, isLoading, error, removeItem, updateQuantity, clearCart } = useCart();

  if (isLoading) {
    return (
      <div className="cart-container">
        <div className="cart-header">
          <h1>Shopping Cart</h1>
          <Link to="/" className="back-link">← Back to Products</Link>
        </div>
        <div className="empty-cart">
          <p>Loading your cart...</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="cart-container">
        <div className="cart-header">
          <h1>Shopping Cart</h1>
          <Link to="/" className="back-link">← Back to Products</Link>
        </div>
        <div className="empty-cart">
          <p style={{ color: 'red' }}>Error loading cart: {error}</p>
          <Link to="/" className="continue-shopping-btn">Back to Products</Link>
        </div>
      </div>
    );
  }

  if (items.length === 0) {
    return (
      <div className="cart-container">
        <div className="cart-header">
          <h1>Shopping Cart</h1>
          <Link to="/" className="back-link">← Back to Products</Link>
        </div>
        <div className="empty-cart">
          <p>No items in Cart</p>
          <Link to="/" className="continue-shopping-btn">Continue Shopping</Link>
        </div>
      </div>
    );
  }

  return (
    <CartPage
      items={items}
      itemCount={itemCount}
      subtotal={subtotal}
      total={total}
      onRemove={removeItem}
      onUpdateQuantity={updateQuantity}
      onClearCart={clearCart}
    />
  );
}

export default Cart;
