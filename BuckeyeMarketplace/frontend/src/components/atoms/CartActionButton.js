import React from 'react';
import { useCart } from '../../context/CartContext';
import { useNotification } from '../../context/NotificationContext';

function CartActionButton({ product }) {
  const { addItem } = useCart();
  const { addNotification } = useNotification();

  const handleAddToCart = (e) => {
    e.preventDefault();
    addItem(product, 1);
    addNotification(`${product.title} added to cart!`, 'success', 3000);
  };

  return (
    <button onClick={handleAddToCart} className="add-to-cart-btn">
      Add to Cart
    </button>
  );
}

export default CartActionButton;
