import React from 'react';
import AddToCartButton from '../atoms/AddToCartButton';
import CartActionButton from '../atoms/CartActionButton';
import PriceDisplay from '../atoms/PriceDisplay';
import ProductMeta from './ProductMeta';

function ProductCardDetails({ product, title, category, sellerName, price }) {
  const isOutOfStock = product.stockQuantity <= 0;

  return (
    <div className="product-details">
      <h2>{title}</h2>
      <ProductMeta category={category} sellerName={sellerName} />

      {isOutOfStock && (
        <div style={{ color: '#d9534f', fontWeight: 'bold', marginBottom: '10px', fontSize: '12px' }}>
          Out of Stock
        </div>
      )}

      <div className="product-footer">
        <PriceDisplay price={price} />
      </div>

      <div className="product-actions">
        {!isOutOfStock && <CartActionButton product={product} />}
        <AddToCartButton label="View Details" />
      </div>
    </div>
  );
}

export default ProductCardDetails;
