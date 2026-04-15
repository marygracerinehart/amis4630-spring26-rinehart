import React from 'react';

/**
 * FormButton - Reusable submit button atom with loading state
 */
function FormButton({ children, type = 'submit', disabled = false, loading = false, className = '' }) {
  return (
    <button
      type={type}
      className={`form-button ${className}`}
      disabled={disabled || loading}
    >
      {loading ? 'Please wait...' : children}
    </button>
  );
}

export default FormButton;
