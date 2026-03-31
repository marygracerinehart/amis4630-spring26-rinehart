import React from 'react';
import { Link } from 'react-router-dom';
import ProductImage from '../atoms/ProductImage';
import ProductCardDetails from '../molecules/ProductCardDetails';

function ProductCard({ product }) {
  return (
    <Link to={`/product/${product.id}`} className="product-card-link">
      <div className="product-card">
        <ProductImage src={product.imageUrl} alt={product.title} />
        <ProductCardDetails
          product={product}
          title={product.title}
          category={product.category}
          sellerName={product.sellerName}
          price={product.price}
        />
      </div>
    </Link>
  );
}

export default ProductCard;
