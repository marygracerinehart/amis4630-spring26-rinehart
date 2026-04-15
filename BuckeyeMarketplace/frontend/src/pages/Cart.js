import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useCart, CART_ACTIONS } from '../context/CartContext';
import { useNotification } from '../context/NotificationContext';
import { placeOrder } from '../services/orderService';
import CartPage from '../components/organisms/CartPage';
import '../styles/Cart.css';

function Cart() {
  const { items, itemCount, subtotal, total, isLoading, error, removeItem, updateQuantity, clearCart, dispatch } = useCart();
  const { addNotification } = useNotification();
  const navigate = useNavigate();
  const [checkoutLoading, setCheckoutLoading] = useState(false);
  const [checkoutError, setCheckoutError] = useState('');
  const [shippingAddress, setShippingAddress] = useState('');

  const handleCheckout = async () => {
    if (!shippingAddress.trim()) {
      setCheckoutError('Please enter a shipping address.');
      return;
    }
    setCheckoutLoading(true);
    setCheckoutError('');
    try {
      const order = await placeOrder(shippingAddress.trim());
      // Clear cart locally (backend already cleared it)
      dispatch({ type: CART_ACTIONS.SET_CART, payload: [] });
      addNotification(`Order #${order.id} placed successfully!`, 'success');
      navigate('/order-confirmation', { state: { order } });
    } catch (err) {
      setCheckoutError(err.message);
      addNotification(err.message, 'error');
    } finally {
      setCheckoutLoading(false);
    }
  };

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
      onCheckout={handleCheckout}
      checkoutLoading={checkoutLoading}
      checkoutError={checkoutError}
      shippingAddress={shippingAddress}
      onShippingAddressChange={setShippingAddress}
    />
  );
}

export default Cart;
