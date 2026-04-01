import React from 'react';
import { Link } from 'react-router-dom';

function CartSummary({ itemCount, subtotal, total, onClearCart, onCheckout }) {
  return (
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

      <button className="checkout-btn" onClick={onCheckout}>
        Proceed to Checkout
      </button>

      <button onClick={onClearCart} className="clear-cart-btn">
        Clear Cart
      </button>

      <Link to="/" className="continue-shopping-link">
        Continue Shopping
      </Link>
    </div>
  );
}

export default CartSummary;
