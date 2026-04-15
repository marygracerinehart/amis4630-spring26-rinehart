import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { useNotification } from '../context/NotificationContext';
import RegisterForm from '../components/molecules/RegisterForm';
import '../styles/Register.css';

/**
 * Register Page - Dedicated registration page with full sign-up form
 * Uses AuthContext for authentication and NotificationContext for feedback
 */
function Register() {
  const [loading, setLoading] = useState(false);
  const [serverError, setServerError] = useState('');
  const { register, isAuthenticated } = useAuth();
  const { addNotification } = useNotification();
  const navigate = useNavigate();

  // Redirect if already logged in
  if (isAuthenticated) {
    navigate('/', { replace: true });
    return null;
  }

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

  return (
    <div className="register-page">
      <div className="register-card">
        <div className="register-header">
          <h2 className="register-title">🌰 Buckeye Marketplace</h2>
          <p className="register-subtitle">Create your account</p>
        </div>

        <RegisterForm
          onSubmit={handleRegister}
          loading={loading}
          serverError={serverError}
        />

        <div className="register-footer">
          <p className="register-footer-text">
            Already have an account?{' '}
            <Link to="/login" className="register-footer-link">
              Sign In
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
}

export default Register;
