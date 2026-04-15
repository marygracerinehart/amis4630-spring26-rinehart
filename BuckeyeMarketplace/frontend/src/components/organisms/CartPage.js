import React from 'react';
import { Link } from 'react-router-dom';
import CartItem from '../molecules/CartItem';
import CartSummary from './CartContent';
import '../../styles/Cart.css';

function CartPage({ items, itemCount, subtotal, total, onRemove, onUpdateQuantity, onClearCart, onCheckout, checkoutLoading, checkoutError, shippingAddress, onShippingAddressChange }) {
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
            <CartItem
              key={item.id}
              item={item}
              onRemove={onRemove}
              onUpdateQuantity={onUpdateQuantity}
            />
          ))}
        </div>

        <CartSummary
          itemCount={itemCount}
          subtotal={subtotal}
          total={total}
          onClearCart={onClearCart}
          onCheckout={onCheckout}
          checkoutLoading={checkoutLoading}
          checkoutError={checkoutError}
          shippingAddress={shippingAddress}
          onShippingAddressChange={onShippingAddressChange}
        />
      </div>
    </div>
  );
}

export default CartPage;
