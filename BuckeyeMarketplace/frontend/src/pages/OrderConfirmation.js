import React from 'react';
import { Link, useLocation } from 'react-router-dom';
import '../styles/OrderConfirmation.css';

/**
 * OrderConfirmation Page - Displays order success details after checkout
 * Receives the order object via location.state from the Cart page
 */
function OrderConfirmation() {
  const location = useLocation();
  const order = location.state?.order;

  // If someone navigates here directly without an order
  if (!order) {
    return (
      <div className="order-confirmation">
        <div className="order-card">
          <div className="order-empty">
            <h2>No Order Found</h2>
            <p>It looks like you navigated here without placing an order.</p>
            <Link to="/" className="order-btn order-btn--primary">Browse Products</Link>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="order-confirmation">
      <div className="order-card">
        <div className="order-success-icon">✅</div>
        <h1 className="order-success-title">Order Placed Successfully!</h1>
        <p className="order-success-subtitle">
          Thank you for your purchase. Your order has been received and is being processed.
        </p>

        <div className="order-details">
          <div className="order-details-header">
            <h2>Order #{order.id}</h2>
            <span className={`order-status order-status--${order.status?.toLowerCase()}`}>
              {order.status}
            </span>
          </div>

          <div className="order-meta">
            <div className="order-meta-item">
              <span className="order-meta-label">Date</span>
              <span className="order-meta-value">
                {new Date(order.orderDate).toLocaleDateString('en-US', {
                  year: 'numeric',
                  month: 'long',
                  day: 'numeric',
                  hour: '2-digit',
                  minute: '2-digit',
                })}
              </span>
            </div>
            <div className="order-meta-item">
              <span className="order-meta-label">Items</span>
              <span className="order-meta-value">{order.items?.length || 0}</span>
            </div>
            <div className="order-meta-item">
              <span className="order-meta-label">Total</span>
              <span className="order-meta-value order-meta-total">
                ${order.totalAmount?.toFixed(2)}
              </span>
            </div>
          </div>

          {order.shippingAddress && (
            <div className="order-shipping">
              <span className="order-meta-label">Ship To</span>
              <p className="order-shipping-address">{order.shippingAddress}</p>
            </div>
          )}

          <div className="order-items-list">
            <h3>Items Ordered</h3>
            {order.items?.map((item) => (
              <div key={item.id} className="order-item">
                {item.imageUrl && (
                  <img src={item.imageUrl} alt={item.title} className="order-item-image" />
                )}
                <div className="order-item-info">
                  <p className="order-item-title">{item.title}</p>
                  {item.category && <p className="order-item-category">{item.category}</p>}
                  {item.sellerName && <p className="order-item-seller">Sold by: {item.sellerName}</p>}
                </div>
                <div className="order-item-qty">×{item.quantity}</div>
                <div className="order-item-price">
                  ${(item.unitPrice * item.quantity).toFixed(2)}
                </div>
              </div>
            ))}
          </div>
        </div>

        <div className="order-actions">
          <Link to="/" className="order-btn order-btn--primary">Continue Shopping</Link>
        </div>
      </div>
    </div>
  );
}

export default OrderConfirmation;
