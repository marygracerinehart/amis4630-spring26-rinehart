import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { useNotification } from '../context/NotificationContext';
import LoginForm from '../components/molecules/LoginForm';
import RegisterForm from '../components/molecules/RegisterForm';
import '../styles/Login.css';

/**
 * Login Page - Provides login and registration with tab switching
 * Uses AuthContext for authentication and NotificationContext for feedback
 */
function Login() {
  const [activeTab, setActiveTab] = useState('login');
  const [loading, setLoading] = useState(false);
  const [serverError, setServerError] = useState('');
  const { login, register, isAuthenticated } = useAuth();
  const { addNotification } = useNotification();
  const navigate = useNavigate();

  // Redirect if already logged in
  if (isAuthenticated) {
    navigate('/', { replace: true });
    return null;
  }

  const handleLogin = async (email, password) => {
    setLoading(true);
    setServerError('');
    try {
      const user = await login(email, password);
      addNotification(`Welcome back, ${user.fullName}!`, 'success');
      navigate('/');
    } catch (err) {
      setServerError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleRegister = async (email, password, confirmPassword, fullName) => {
    setLoading(true);
    setServerError('');
    try {
      const user = await register(email, password, confirmPassword, fullName);
      addNotification(`Welcome to Buckeye Marketplace, ${user.fullName}!`, 'success');
      navigate('/');
    } catch (err) {
      setServerError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const switchTab = (tab) => {
    setActiveTab(tab);
    setServerError('');
  };

  return (
    <div className="login-page">
      <div className="login-card">
        <div className="login-header">
          <h2 className="login-title">🌰 Buckeye Marketplace</h2>
          <p className="login-subtitle">
            {activeTab === 'login'
              ? 'Sign in to your account'
              : 'Create a new account'}
          </p>
        </div>

        <div className="login-tabs">
          <button
            className={`login-tab ${activeTab === 'login' ? 'login-tab--active' : ''}`}
            onClick={() => switchTab('login')}
            type="button"
          >
            Sign In
          </button>
          <button
            className={`login-tab ${activeTab === 'register' ? 'login-tab--active' : ''}`}
            onClick={() => switchTab('register')}
            type="button"
          >
            Register
          </button>
        </div>

        {activeTab === 'login' ? (
          <LoginForm
            onSubmit={handleLogin}
            loading={loading}
            serverError={serverError}
          />
        ) : (
          <RegisterForm
            onSubmit={handleRegister}
            loading={loading}
            serverError={serverError}
          />
        )}
      </div>
    </div>
  );
}

export default Login;
