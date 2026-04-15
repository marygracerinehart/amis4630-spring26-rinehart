import React from 'react';

/**
 * FormInput - Reusable form input atom with label and error display
 */
function FormInput({ id, label, type = 'text', value, onChange, error, placeholder, required = false, autoComplete }) {
  return (
    <div className="form-group">
      <label htmlFor={id} className="form-label">
        {label}
      </label>
      <input
        id={id}
        type={type}
        className={`form-input ${error ? 'form-input--error' : ''}`}
        value={value}
        onChange={onChange}
        placeholder={placeholder}
        required={required}
        autoComplete={autoComplete}
      />
      {error && <span className="form-error">{error}</span>}
    </div>
  );
}

export default FormInput;
