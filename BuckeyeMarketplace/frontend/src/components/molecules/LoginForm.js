import React, { useState } from 'react';
import FormInput from '../atoms/FormInput';
import FormButton from '../atoms/FormButton';

/**
 * LoginForm - Molecule that composes form inputs and button for login
 */
function LoginForm({ onSubmit, loading, serverError }) {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [errors, setErrors] = useState({});

  const validate = () => {
    const newErrors = {};
    if (!email.trim()) {
      newErrors.email = 'Email is required.';
    } else if (!/\S+@\S+\.\S+/.test(email)) {
      newErrors.email = 'Please enter a valid email address.';
    }
    if (!password) {
      newErrors.password = 'Password is required.';
    }
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    if (validate()) {
      onSubmit(email, password);
    }
  };

  return (
    <form className="login-form" onSubmit={handleSubmit} noValidate>
      {serverError && (
        <div className="form-server-error">{serverError}</div>
      )}

      <FormInput
        id="login-email"
        label="Email"
        type="email"
        value={email}
        onChange={(e) => setEmail(e.target.value)}
        error={errors.email}
        placeholder="you@example.com"
        required
        autoComplete="email"
      />

      <FormInput
        id="login-password"
        label="Password"
        type="password"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        error={errors.password}
        placeholder="Enter your password"
        required
        autoComplete="current-password"
      />

      <FormButton loading={loading}>Sign In</FormButton>
    </form>
  );
}

export default LoginForm;
