import React from 'react';
import { Link } from 'react-router-dom';
import { useCart } from '../context/CartContext';
import { useNotification } from '../context/NotificationContext';
import '../styles/Cart.css';

function Cart() {
  const { items, itemCount, subtotal, total, isLoading, error, removeItem, updateQuantity, clearCart } = useCart();
  const { addNotification } = useNotification();

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
          <p>Your cart is empty</p>
          <Link to="/" className="continue-shopping-btn">Continue Shopping</Link>
        </div>
      </div>
    );
  }

  return (
    <div className="cart-container">
      <div className="cart-header">
        <h1>Shopping Cart</h1>
        <Link to="/" className="back-link">← Back to Products</Link>
      </div>

      <div className="cart-content">
        <div className="cart-items">
          <div className="cart-items-header">
            <span>Product</span>
            <span>Price</span>
            <span>Quantity</span>
            <span>Subtotal</span>
            <span>Action</span>
          </div>

          {items.map((item) => (
            <div key={item.id} className="cart-item">
              <div className="item-product">
                <img src={item.imageUrl} alt={item.title} className="item-image" />
                <div className="item-info">
                  <h3>{item.title}</h3>
                  <p className="item-category">{item.category}</p>
                  {item.sellerName && <p className="item-seller">By: {item.sellerName}</p>}
                </div>
              </div>

              <div className="item-price">
                ${item.price.toFixed(2)}
              </div>

              <div className="item-quantity">
                <button
                  onClick={() => updateQuantity(item.id, item.quantity - 1)}
                  className="qty-btn"
                  disabled={item.quantity <= 1}
                >
                  −
                </button>
                <input
                  type="number"
                  value={item.quantity}
                  onChange={(e) => updateQuantity(item.id, parseInt(e.target.value) || 1)}
                  className="qty-input"
                  min="1"
                />
                <button
                  onClick={() => updateQuantity(item.id, item.quantity + 1)}
                  className="qty-btn"
                >
                  +
                </button>
              </div>

              <div className="item-subtotal">
                ${(item.price * item.quantity).toFixed(2)}
              </div>

              <button
                onClick={async () => {
                  const productTitle = await removeItem(item.id);
                  if (productTitle) {
                    addNotification(`${productTitle} removed from cart`, 'error', 3000);
                  }
                }}
                className="remove-btn"
                title="Remove from cart"
              >
                ✕
              </button>
            </div>
          ))}
        </div>

        <div className="cart-summary">
          <div className="summary-header">Order Summary</div>

          <div className="summary-row">
            <span>Items ({itemCount}):</span>
            <span>${subtotal.toFixed(2)}</span>
          </div>

          <div className="summary-row">
            <span>Shipping:</span>
            <span>Calculated at checkout</span>
          </div>

          <div className="summary-row">
            <span>Tax:</span>
            <span>Calculated at checkout</span>
          </div>

          <div className="summary-divider"></div>

          <div className="summary-total">
            <span>Total:</span>
            <span>${total.toFixed(2)}</span>
          </div>

          <button className="checkout-btn">Proceed to Checkout</button>

          <button onClick={clearCart} className="clear-cart-btn">
            Clear Cart
          </button>

          <Link to="/" className="continue-shopping-link">
            Continue Shopping
          </Link>
        </div>
      </div>
    </div>
  );
}

export default Cart;
