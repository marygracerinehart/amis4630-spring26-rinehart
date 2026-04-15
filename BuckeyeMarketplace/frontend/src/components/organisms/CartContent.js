import React from 'react';
import { Link } from 'react-router-dom';

function CartSummary({ itemCount, subtotal, total, onClearCart, onCheckout, checkoutLoading, checkoutError, shippingAddress, onShippingAddressChange }) {
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

      <div className="shipping-address-group">
        <label htmlFor="shipping-address" className="shipping-address-label">
          Shipping Address
        </label>
        <textarea
          id="shipping-address"
          className="shipping-address-input"
          placeholder="Enter your full shipping address..."
          value={shippingAddress}
          onChange={(e) => onShippingAddressChange(e.target.value)}
          rows={3}
          disabled={checkoutLoading}
        />
      </div>

      {checkoutError && (
        <div className="checkout-error">{checkoutError}</div>
      )}

      <button
        className="checkout-btn"
        onClick={onCheckout}
        disabled={checkoutLoading}
      >
        {checkoutLoading ? 'Placing Order...' : 'Place Order'}
      </button>

      <button onClick={onClearCart} className="clear-cart-btn" disabled={checkoutLoading}>
        Clear Cart
      </button>

      <Link to="/" className="continue-shopping-link">
        Continue Shopping
      </Link>
    </div>
  );
}

export default CartSummary;
