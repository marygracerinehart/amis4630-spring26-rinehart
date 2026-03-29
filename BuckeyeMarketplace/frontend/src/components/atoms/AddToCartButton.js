import React from 'react';

function AddToCartButton({ label = 'Add to Cart' }) {
  return <button className="add-to-cart-btn">{label}</button>;
}

export default AddToCartButton;
