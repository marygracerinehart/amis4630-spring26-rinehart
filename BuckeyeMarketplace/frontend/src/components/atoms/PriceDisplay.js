import React from 'react';

function PriceDisplay({ price }) {
  const normalizedPrice = Number(price);
  const safePrice = Number.isFinite(normalizedPrice) ? normalizedPrice : 0;

  return <div className="price">${safePrice.toFixed(2)}</div>;
}

export default PriceDisplay;
