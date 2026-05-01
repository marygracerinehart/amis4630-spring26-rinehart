import React, { useState, useEffect } from 'react';
import '../styles/ProductList.css';
import ProductCard from '../components/organisms/ProductCard';
import { fetchAllProducts } from '../services/productService';

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
      const data = await fetchAllProducts();
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
            <ProductCard key={product.id} product={product} />
          ))
        ) : (
          <p>No products available</p>
        )}
      </div>
    </div>
  );
}

export default ProductList;
