import React from 'react';
import AddToCartButton from '../atoms/AddToCartButton';
import CartActionButton from '../atoms/CartActionButton';
import PriceDisplay from '../atoms/PriceDisplay';
import ProductMeta from './ProductMeta';

function ProductCardDetails({ product, title, category, sellerName, price }) {
  return (
    <div className="product-details">
      <h2>{title}</h2>
      <ProductMeta category={category} sellerName={sellerName} />

      <div className="product-footer">
        <PriceDisplay price={price} />
      </div>

      <div className="product-actions">
        <CartActionButton product={product} />
        <AddToCartButton label="View Details" />
      </div>
    </div>
  );
}

export default ProductCardDetails;
