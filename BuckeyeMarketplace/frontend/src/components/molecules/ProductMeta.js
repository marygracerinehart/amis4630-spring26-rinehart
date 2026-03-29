import React from 'react';

function ProductMeta({ category, sellerName }) {
  return (
    <>
      <p className="category">{category}</p>
      <p className="seller-name">By: {sellerName}</p>
    </>
  );
}

export default ProductMeta;
