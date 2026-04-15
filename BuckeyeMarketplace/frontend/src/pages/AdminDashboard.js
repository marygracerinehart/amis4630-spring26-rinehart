import React, { useState, useEffect, useCallback } from 'react';
import { useNotification } from '../context/NotificationContext';
import {
  getDashboardStats,
  getUsers,
  getAllOrders,
  updateOrderStatus,
  deleteUser,
  getProducts,
  createProduct,
  updateProduct,
  deleteProduct,
} from '../services/adminService';
import '../styles/AdminDashboard.css';

/**
 * AdminDashboard Page — Admin-only dashboard showing marketplace stats,
 * recent orders, and user management. Protected by ProtectedRoute roles={['Admin']}.
 */
function AdminDashboard() {
  const { addNotification } = useNotification();

  // Active tab
  const [activeTab, setActiveTab] = useState('overview');

  // Dashboard stats
  const [stats, setStats] = useState(null);
  const [statsLoading, setStatsLoading] = useState(true);

  // Orders
  const [orders, setOrders] = useState([]);
  const [ordersLoading, setOrdersLoading] = useState(false);

  // Users
  const [users, setUsers] = useState([]);
  const [usersLoading, setUsersLoading] = useState(false);

  // Products
  const [products, setProducts] = useState([]);
  const [productsLoading, setProductsLoading] = useState(false);
  const [showProductForm, setShowProductForm] = useState(false);
  const [editingProduct, setEditingProduct] = useState(null);

  // ── Fetch dashboard stats ──────────────────────────────
  const loadStats = useCallback(async () => {
    setStatsLoading(true);
    try {
      const data = await getDashboardStats();
      setStats(data);
    } catch (err) {
      addNotification(err.message, 'error');
    } finally {
      setStatsLoading(false);
    }
  }, [addNotification]);

  // ── Fetch all orders ───────────────────────────────────
  const loadOrders = useCallback(async () => {
    setOrdersLoading(true);
    try {
      const data = await getAllOrders();
      setOrders(data);
    } catch (err) {
      addNotification(err.message, 'error');
    } finally {
      setOrdersLoading(false);
    }
  }, [addNotification]);

  // ── Fetch all users ────────────────────────────────────
  const loadUsers = useCallback(async () => {
    setUsersLoading(true);
    try {
      const data = await getUsers();
      setUsers(data);
    } catch (err) {
      addNotification(err.message, 'error');
    } finally {
      setUsersLoading(false);
    }
  }, [addNotification]);

  // ── Fetch all products ─────────────────────────────────
  const loadProducts = useCallback(async () => {
    setProductsLoading(true);
    try {
      const data = await getProducts();
      setProducts(data);
    } catch (err) {
      addNotification(err.message, 'error');
    } finally {
      setProductsLoading(false);
    }
  }, [addNotification]);

  // Load stats on mount
  useEffect(() => {
    loadStats();
  }, [loadStats]);

  // Load tab data when switching
  useEffect(() => {
    if (activeTab === 'orders' && orders.length === 0) loadOrders();
    if (activeTab === 'users' && users.length === 0) loadUsers();
    if (activeTab === 'products' && products.length === 0) loadProducts();
  }, [activeTab, orders.length, users.length, products.length, loadOrders, loadUsers, loadProducts]);

  // ── Order status update ────────────────────────────────
  const handleStatusChange = async (orderId, newStatus) => {
    try {
      await updateOrderStatus(orderId, newStatus);
      setOrders((prev) =>
        prev.map((o) => (o.id === orderId ? { ...o, status: newStatus } : o))
      );
      addNotification(`Order #${orderId} updated to "${newStatus}"`, 'success');
    } catch (err) {
      addNotification(err.message, 'error');
    }
  };

  // ── Delete user ────────────────────────────────────────
  const handleDeleteUser = async (userId, email) => {
    if (!window.confirm(`Are you sure you want to delete ${email}?`)) return;

    try {
      await deleteUser(userId);
      setUsers((prev) => prev.filter((u) => u.id !== userId));
      addNotification(`User "${email}" deleted.`, 'success');
      // Refresh stats since user count changed
      loadStats();
    } catch (err) {
      addNotification(err.message, 'error');
    }
  };

  // ── Product management handlers ────────────────────────
  const handleAddProduct = () => {
    setEditingProduct(null);
    setShowProductForm(true);
  };

  const handleEditProduct = (product) => {
    setEditingProduct(product);
    setShowProductForm(true);
  };

  const handleDeleteProduct = async (productId, title) => {
    if (!window.confirm(`Are you sure you want to delete "${title}"?`)) return;

    try {
      await deleteProduct(productId);
      setProducts((prev) => prev.filter((p) => p.id !== productId));
      addNotification(`Product "${title}" deleted.`, 'success');
      loadStats();
    } catch (err) {
      addNotification(err.message, 'error');
    }
  };

  const handleProductFormSubmit = async (productData) => {
    try {
      if (editingProduct) {
        // Update existing product
        await updateProduct(editingProduct.id, { ...productData, id: editingProduct.id });
        setProducts((prev) =>
          prev.map((p) => (p.id === editingProduct.id ? { ...p, ...productData } : p))
        );
        addNotification(`Product "${productData.title}" updated.`, 'success');
      } else {
        // Create new product
        const newProduct = await createProduct(productData);
        setProducts((prev) => [...prev, newProduct]);
        addNotification(`Product "${productData.title}" created.`, 'success');
      }
      setShowProductForm(false);
      setEditingProduct(null);
      loadStats();
    } catch (err) {
      addNotification(err.message, 'error');
    }
  };

  const handleProductFormCancel = () => {
    setShowProductForm(false);
    setEditingProduct(null);
  };

  // ══════════════════════════════════════════════════════
  //  RENDER
  // ══════════════════════════════════════════════════════

  return (
    <div className="admin-dashboard">
      <div className="admin-header">
        <h1 className="admin-title">Admin Dashboard</h1>
        <p className="admin-subtitle">Manage your marketplace</p>
      </div>

      {/* Tab Navigation */}
      <div className="admin-tabs">
        <button
          className={`admin-tab ${activeTab === 'overview' ? 'admin-tab--active' : ''}`}
          onClick={() => setActiveTab('overview')}
        >
          Overview
        </button>
        <button
          className={`admin-tab ${activeTab === 'orders' ? 'admin-tab--active' : ''}`}
          onClick={() => setActiveTab('orders')}
        >
          Orders
        </button>
        <button
          className={`admin-tab ${activeTab === 'users' ? 'admin-tab--active' : ''}`}
          onClick={() => setActiveTab('users')}
        >
          Users
        </button>
        <button
          className={`admin-tab ${activeTab === 'products' ? 'admin-tab--active' : ''}`}
          onClick={() => setActiveTab('products')}
        >
          Products
        </button>
      </div>

      {/* Tab Content */}
      <div className="admin-content">
        {activeTab === 'overview' && (
          <OverviewTab stats={stats} loading={statsLoading} />
        )}
        {activeTab === 'orders' && (
          <OrdersTab orders={orders} loading={ordersLoading} onStatusChange={handleStatusChange} onRefresh={loadOrders} />
        )}
        {activeTab === 'users' && (
          <UsersTab users={users} loading={usersLoading} onDeleteUser={handleDeleteUser} />
        )}
        {activeTab === 'products' && (
          <ProductsTab
            products={products}
            loading={productsLoading}
            showForm={showProductForm}
            editingProduct={editingProduct}
            onAdd={handleAddProduct}
            onEdit={handleEditProduct}
            onDelete={handleDeleteProduct}
            onFormSubmit={handleProductFormSubmit}
            onFormCancel={handleProductFormCancel}
          />
        )}
      </div>
    </div>
  );
}

// ══════════════════════════════════════════════════════════
//  OVERVIEW TAB
// ══════════════════════════════════════════════════════════

function OverviewTab({ stats, loading }) {
  if (loading) {
    return <div className="admin-loading">Loading dashboard stats...</div>;
  }

  if (!stats) {
    return <div className="admin-empty">Unable to load statistics.</div>;
  }

  const cards = [
    { label: 'Total Users', value: stats.totalUsers, icon: '👥', color: '#3498db' },
    { label: 'Total Products', value: stats.totalProducts, icon: '📦', color: '#27ae60' },
    { label: 'Total Orders', value: stats.totalOrders, icon: '🛒', color: '#9b59b6' },
    { label: 'Revenue', value: `$${stats.totalRevenue?.toFixed(2)}`, icon: '💰', color: '#f39c12' },
    { label: 'Pending Orders', value: stats.pendingOrders, icon: '⏳', color: '#e74c3c' },
  ];

  return (
    <div className="admin-stats-grid">
      {cards.map((card) => (
        <div key={card.label} className="admin-stat-card" style={{ borderTopColor: card.color }}>
          <div className="stat-icon">{card.icon}</div>
          <div className="stat-value">{card.value}</div>
          <div className="stat-label">{card.label}</div>
        </div>
      ))}
    </div>
  );
}

// ══════════════════════════════════════════════════════════
//  ORDERS TAB
// ══════════════════════════════════════════════════════════

function OrdersTab({ orders, loading, onStatusChange, onRefresh }) {
  const [filterStatus, setFilterStatus] = useState('All');
  const [expandedOrderId, setExpandedOrderId] = useState(null);

  if (loading) {
    return <div className="admin-loading">Loading orders...</div>;
  }

  const statusOptions = ['Pending', 'Processing', 'Shipped', 'Delivered', 'Cancelled'];
  const filterOptions = ['All', ...statusOptions];

  const filteredOrders = filterStatus === 'All'
    ? orders
    : orders.filter((o) => o.status === filterStatus);

  const toggleExpand = (orderId) => {
    setExpandedOrderId((prev) => (prev === orderId ? null : orderId));
  };

  return (
    <div>
      <div className="admin-section-header">
        <h2 className="admin-section-title">Orders ({filteredOrders.length})</h2>
        <div className="orders-toolbar">
          <label className="orders-filter-label">
            Filter:
            <select
              className="orders-filter-select"
              value={filterStatus}
              onChange={(e) => setFilterStatus(e.target.value)}
            >
              {filterOptions.map((s) => (
                <option key={s} value={s}>{s}</option>
              ))}
            </select>
          </label>
          <button className="admin-refresh-btn" onClick={onRefresh} title="Refresh orders">
            ↻ Refresh
          </button>
        </div>
      </div>

      {filteredOrders.length === 0 ? (
        <div className="admin-empty">No orders match the selected filter.</div>
      ) : (
        <div className="admin-table-wrapper">
          <table className="admin-table">
            <thead>
              <tr>
                <th></th>
                <th>Order #</th>
                <th>User ID</th>
                <th>Date</th>
                <th>Items</th>
                <th>Total</th>
                <th>Address</th>
                <th>Status</th>
              </tr>
            </thead>
            <tbody>
              {filteredOrders.map((order) => (
                <React.Fragment key={order.id}>
                  <tr className={expandedOrderId === order.id ? 'order-row--expanded' : ''}>
                    <td>
                      <button
                        className="order-expand-btn"
                        onClick={() => toggleExpand(order.id)}
                        title={expandedOrderId === order.id ? 'Collapse' : 'View items'}
                      >
                        {expandedOrderId === order.id ? '▾' : '▸'}
                      </button>
                    </td>
                    <td className="order-id-cell">#{order.id}</td>
                    <td className="user-id-cell" title={order.userId}>
                      {order.userId?.substring(0, 8)}...
                    </td>
                    <td>{new Date(order.orderDate).toLocaleDateString()}</td>
                    <td>{order.items?.length || 0}</td>
                    <td className="total-cell">${order.totalAmount?.toFixed(2)}</td>
                    <td className="address-cell" title={order.shippingAddress}>
                      {order.shippingAddress?.substring(0, 30)}{order.shippingAddress?.length > 30 ? '...' : ''}
                    </td>
                    <td>
                      <select
                        className={`status-select status-select--${order.status?.toLowerCase()}`}
                        value={order.status}
                        onChange={(e) => onStatusChange(order.id, e.target.value)}
                      >
                        {statusOptions.map((s) => (
                          <option key={s} value={s}>{s}</option>
                        ))}
                      </select>
                    </td>
                  </tr>
                  {expandedOrderId === order.id && (
                    <tr className="order-detail-row">
                      <td colSpan="8">
                        <div className="order-detail-content">
                          <h4 className="order-detail-heading">Order Items</h4>
                          {order.items && order.items.length > 0 ? (
                            <table className="order-items-table">
                              <thead>
                                <tr>
                                  <th>Product</th>
                                  <th>Category</th>
                                  <th>Qty</th>
                                  <th>Unit Price</th>
                                  <th>Subtotal</th>
                                </tr>
                              </thead>
                              <tbody>
                                {order.items.map((item) => (
                                  <tr key={item.id}>
                                    <td className="user-name-cell">{item.title}</td>
                                    <td>{item.category || '—'}</td>
                                    <td>{item.quantity}</td>
                                    <td>${item.unitPrice?.toFixed(2)}</td>
                                    <td className="total-cell">${(item.unitPrice * item.quantity)?.toFixed(2)}</td>
                                  </tr>
                                ))}
                              </tbody>
                            </table>
                          ) : (
                            <p className="order-detail-empty">No items in this order.</p>
                          )}
                          <div className="order-detail-ship">
                            <strong>Ship To:</strong> {order.shippingAddress || '—'}
                          </div>
                        </div>
                      </td>
                    </tr>
                  )}
                </React.Fragment>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}

// ══════════════════════════════════════════════════════════
//  USERS TAB
// ══════════════════════════════════════════════════════════

function UsersTab({ users, loading, onDeleteUser }) {
  if (loading) {
    return <div className="admin-loading">Loading users...</div>;
  }

  if (users.length === 0) {
    return <div className="admin-empty">No users found.</div>;
  }

  return (
    <div className="admin-table-wrapper">
      <table className="admin-table">
        <thead>
          <tr>
            <th>Name</th>
            <th>Email</th>
            <th>Roles</th>
            <th>Joined</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {users.map((user) => (
            <tr key={user.id}>
              <td className="user-name-cell">{user.fullName || '—'}</td>
              <td>{user.email}</td>
              <td>
                {user.roles?.map((role) => (
                  <span key={role} className={`role-badge role-badge--${role.toLowerCase()}`}>
                    {role}
                  </span>
                ))}
              </td>
              <td>{new Date(user.createdAt).toLocaleDateString()}</td>
              <td>
                <button
                  className="admin-delete-btn"
                  onClick={() => onDeleteUser(user.id, user.email)}
                  disabled={user.roles?.includes('Admin')}
                  title={user.roles?.includes('Admin') ? 'Cannot delete admin users' : `Delete ${user.email}`}
                >
                  Delete
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

// ══════════════════════════════════════════════════════════
//  PRODUCTS TAB
// ══════════════════════════════════════════════════════════

function ProductsTab({ products, loading, showForm, editingProduct, onAdd, onEdit, onDelete, onFormSubmit, onFormCancel }) {
  if (loading) {
    return <div className="admin-loading">Loading products...</div>;
  }

  return (
    <div>
      <div className="admin-section-header">
        <h2 className="admin-section-title">Products ({products.length})</h2>
        <button className="admin-add-btn" onClick={onAdd}>
          + Add Product
        </button>
      </div>

      {showForm && (
        <ProductForm
          product={editingProduct}
          onSubmit={onFormSubmit}
          onCancel={onFormCancel}
        />
      )}

      {products.length === 0 && !showForm ? (
        <div className="admin-empty">No products found. Click &quot;+ Add Product&quot; to create one.</div>
      ) : (
        <div className="admin-table-wrapper">
          <table className="admin-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Title</th>
                <th>Category</th>
                <th>Price</th>
                <th>Stock</th>
                <th>Seller</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {products.map((product) => (
                <tr key={product.id}>
                  <td className="order-id-cell">#{product.id}</td>
                  <td className="user-name-cell">{product.title}</td>
                  <td>{product.category || '—'}</td>
                  <td className="total-cell">${product.price?.toFixed(2)}</td>
                  <td>{product.stockQuantity ?? 0}</td>
                  <td>{product.sellerName || '—'}</td>
                  <td className="product-actions-cell">
                    <button className="admin-edit-btn" onClick={() => onEdit(product)}>
                      Edit
                    </button>
                    <button
                      className="admin-delete-btn"
                      onClick={() => onDelete(product.id, product.title)}
                    >
                      Delete
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}

// ══════════════════════════════════════════════════════════
//  PRODUCT FORM (Add / Edit)
// ══════════════════════════════════════════════════════════

const EMPTY_PRODUCT = {
  title: '',
  description: '',
  category: '',
  price: '',
  sellerName: '',
  imageUrl: '',
  stockQuantity: '10',
};

function ProductForm({ product, onSubmit, onCancel }) {
  const [formData, setFormData] = useState(
    product
      ? {
          title: product.title || '',
          description: product.description || '',
          category: product.category || '',
          price: product.price?.toString() || '',
          sellerName: product.sellerName || '',
          imageUrl: product.imageUrl || '',
          stockQuantity: product.stockQuantity?.toString() || '10',
        }
      : { ...EMPTY_PRODUCT }
  );

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = (e) => {
    e.preventDefault();

    if (!formData.title.trim()) return;
    if (!formData.price || isNaN(Number(formData.price)) || Number(formData.price) < 0) return;

    onSubmit({
      title: formData.title.trim(),
      description: formData.description.trim(),
      category: formData.category.trim(),
      price: parseFloat(formData.price),
      sellerName: formData.sellerName.trim(),
      imageUrl: formData.imageUrl.trim(),
      stockQuantity: parseInt(formData.stockQuantity, 10) || 0,
      postedDate: product?.postedDate || new Date().toISOString(),
    });
  };

  return (
    <div className="product-form-wrapper">
      <h3 className="product-form-title">
        {product ? 'Edit Product' : 'Add New Product'}
      </h3>
      <form className="product-form" onSubmit={handleSubmit}>
        <div className="product-form-grid">
          <div className="product-form-group">
            <label htmlFor="pf-title">Title *</label>
            <input id="pf-title" name="title" value={formData.title} onChange={handleChange} required />
          </div>
          <div className="product-form-group">
            <label htmlFor="pf-category">Category</label>
            <input id="pf-category" name="category" value={formData.category} onChange={handleChange} />
          </div>
          <div className="product-form-group">
            <label htmlFor="pf-price">Price *</label>
            <input id="pf-price" name="price" type="number" step="0.01" min="0" value={formData.price} onChange={handleChange} required />
          </div>
          <div className="product-form-group">
            <label htmlFor="pf-stock">Stock Quantity</label>
            <input id="pf-stock" name="stockQuantity" type="number" min="0" value={formData.stockQuantity} onChange={handleChange} />
          </div>
          <div className="product-form-group">
            <label htmlFor="pf-seller">Seller Name</label>
            <input id="pf-seller" name="sellerName" value={formData.sellerName} onChange={handleChange} />
          </div>
          <div className="product-form-group">
            <label htmlFor="pf-image">Image URL</label>
            <input id="pf-image" name="imageUrl" value={formData.imageUrl} onChange={handleChange} />
          </div>
          <div className="product-form-group product-form-group--full">
            <label htmlFor="pf-desc">Description</label>
            <textarea id="pf-desc" name="description" rows="3" value={formData.description} onChange={handleChange} />
          </div>
        </div>
        <div className="product-form-actions">
          <button type="submit" className="admin-save-btn">
            {product ? 'Save Changes' : 'Create Product'}
          </button>
          <button type="button" className="admin-cancel-btn" onClick={onCancel}>
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
}

export default AdminDashboard;
