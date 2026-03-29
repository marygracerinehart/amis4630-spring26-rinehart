import React from 'react';

function ProductImage({ src, alt }) {
  return (
    <div className="product-image">
      <img src={src} alt={alt} />
    </div>
  );
}

export default ProductImage;
