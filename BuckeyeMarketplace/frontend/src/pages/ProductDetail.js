import React, { useState, useEffect, useCallback } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useCart } from '../context/CartContext';
import { useNotification } from '../context/NotificationContext';
import '../styles/ProductDetail.css';
import { fetchProductById } from '../services/productService';

function ProductDetail() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { addItem } = useCart();
  const { addNotification } = useNotification();
  const [product, setProduct] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const fetchProduct = useCallback(async () => {
    try {
      setLoading(true);
      const data = await fetchProductById(id);
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

  const handleAddToCart = (e) => {
    e.preventDefault();
    if (product) {
      addItem(product, 1);
      addNotification(`${product.title} added to cart!`, 'success', 3000);
    }
  };

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
          
          <div className="detail-stock">
            <strong>Stock:</strong>
            {product.stockQuantity > 0 ? (
              <span style={{ color: '#5cb85c', marginLeft: '8px' }}>
                {product.stockQuantity} available
              </span>
            ) : (
              <span style={{ color: '#d9534f', marginLeft: '8px', fontWeight: 'bold' }}>
                Out of Stock
              </span>
            )}
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
            <button 
              onClick={handleAddToCart} 
              className="add-to-cart-btn-detail"
              disabled={product.stockQuantity <= 0}
              style={{ opacity: product.stockQuantity <= 0 ? 0.5 : 1, cursor: product.stockQuantity <= 0 ? 'not-allowed' : 'pointer' }}
            >
              {product.stockQuantity <= 0 ? 'Out of Stock' : 'Add to Cart'}
            </button>
            <button className="contact-seller-btn">Contact Seller</button>
          </div>
        </div>
      </div>
    </div>
  );
}

export default ProductDetail;
