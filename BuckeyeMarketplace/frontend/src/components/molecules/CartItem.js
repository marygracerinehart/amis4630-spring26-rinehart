import React from 'react';
import { useNotification } from '../../context/NotificationContext';

function CartItem({ item, onRemove, onUpdateQuantity }) {
  const { addNotification } = useNotification();

  const handleRemove = async () => {
    const productTitle = await onRemove(item.id);
    if (productTitle) {
      addNotification(`${productTitle} removed from cart`, 'error', 3000);
    }
  };

  return (
    <div className="cart-item">
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
          onClick={() => onUpdateQuantity(item.id, item.quantity - 1)}
          className="qty-btn"
          disabled={item.quantity <= 1}
        >
          −
        </button>
        <input
          type="number"
          value={item.quantity}
          onChange={(e) => onUpdateQuantity(item.id, parseInt(e.target.value) || 1)}
          className="qty-input"
          min="1"
        />
        <button
          onClick={() => onUpdateQuantity(item.id, item.quantity + 1)}
          className="qty-btn"
        >
          +
        </button>
      </div>

      <div className="item-subtotal">
        ${(item.price * item.quantity).toFixed(2)}
      </div>

      <button
        onClick={handleRemove}
        className="remove-btn"
        title="Remove from cart"
      >
        ✕
      </button>
    </div>
  );
}

export default CartItem;
