import React, { useState, useEffect, useCallback } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import '../styles/ProductDetail.css';

function ProductDetail() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [product, setProduct] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const fetchProduct = useCallback(async () => {
    try {
      setLoading(true);
      const apiUrl = process.env.REACT_APP_API_URL || 'http://localhost:5107';
      const response = await fetch(`${apiUrl}/api/products/${id}`);
      if (!response.ok) {
        throw new Error('Failed to fetch product');
      }
      const data = await response.json();
      setProduct(data);
      setError(null);
    } catch (err) {
      setError(err.message);
      console.error('Error fetching product:', err);
    } finally {
      setLoading(false);
    }
  }, [id]);

  useEffect(() => {
    fetchProduct();
  }, [fetchProduct]);

  if (loading) {
    return <div className="product-detail-container"><p>Loading product...</p></div>;
  }

  if (error) {
    return (
      <div className="product-detail-container">
        <p className="error">Error: {error}</p>
        <button onClick={() => navigate('/')} className="back-btn">Back to Products</button>
      </div>
    );
  }

  if (!product) {
    return (
      <div className="product-detail-container">
        <p>Product not found</p>
        <button onClick={() => navigate('/')} className="back-btn">Back to Products</button>
      </div>
    );
  }

  return (
    <div className="product-detail-container">
      <button onClick={() => navigate('/')} className="back-btn">← Back to Products</button>
      
      <div className="product-detail-content">
        <div className="product-detail-image">
          <img src={product.imageUrl} alt={product.title} />
        </div>
        
        <div className="product-detail-info">
          <h1>{product.title}</h1>
          
          <div className="detail-category">
            <strong>Category:</strong> {product.category}
          </div>
          
          <div className="detail-price">
            <span className="price-label">Price:</span>
            <span className="price-value">${product.price.toFixed(2)}</span>
          </div>
          
          <div className="detail-description">
            <strong>Description:</strong>
            <p>{product.description}</p>
          </div>
          
          <div className="detail-seller">
            <strong>Seller:</strong> {product.sellerName}
          </div>
          
          <div className="detail-posted">
            <strong>Posted:</strong> {new Date(product.postedDate).toLocaleDateString()}
          </div>
          
          <div className="product-detail-actions">
            <button className="add-to-cart-btn-detail">Add to Cart</button>
            <button className="contact-seller-btn">Contact Seller</button>
          </div>
        </div>
      </div>
    </div>
  );
}

export default ProductDetail;
