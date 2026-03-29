import React from 'react';
import AddToCartButton from '../atoms/AddToCartButton';
import PriceDisplay from '../atoms/PriceDisplay';
import ProductMeta from './ProductMeta';

function ProductCardDetails({ title, category, sellerName, price }) {
  return (
    <div className="product-details">
      <h2>{title}</h2>
      <ProductMeta category={category} sellerName={sellerName} />

      <div className="product-footer">
        <PriceDisplay price={price} />
      </div>

      <AddToCartButton />
    </div>
  );
}

export default ProductCardDetails;
