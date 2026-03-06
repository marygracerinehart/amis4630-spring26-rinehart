import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import '../styles/ProductList.css';

function ProductList() {
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    fetchProducts();
  }, []);

  const fetchProducts = async () => {
    try {
      setLoading(true);
      const response = await fetch('http://localhost:5107/api/products');
      if (!response.ok) {
        throw new Error('Failed to fetch products');
      }
      const data = await response.json();
      setProducts(data);
      setError(null);
    } catch (err) {
      setError(err.message);
      console.error('Error fetching products:', err);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return <div className="product-list-container"><p>Loading products...</p></div>;
  }

  if (error) {
    return <div className="product-list-container"><p className="error">Error: {error}</p></div>;
  }

  return (
    <div className="product-list-container">
      <h1>Available Products</h1>
      <div className="products-grid">
        {products.length > 0 ? (
          products.map((product) => (
            <Link key={product.id} to={`/product/${product.id}`} className="product-card-link">
              <div className="product-card">
                <div className="product-image">
                  <img src={product.imageUrl} alt={product.title} />
                </div>
                <div className="product-details">
                  <h2>{product.title}</h2>
                  <p className="category">{product.category}</p>
                  <p className="seller-name">By: {product.sellerName}</p>
                  <div className="product-footer">
                    <div className="price">${product.price.toFixed(2)}</div>
                  </div>
                  <button className="add-to-cart-btn">Add to Cart</button>
                </div>
              </div>
            </Link>
          ))
        ) : (
          <p>No products available</p>
        )}
      </div>
    </div>
  );
}

export default ProductList;
