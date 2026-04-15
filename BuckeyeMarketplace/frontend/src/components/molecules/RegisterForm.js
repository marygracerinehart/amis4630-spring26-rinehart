import React, { useState } from 'react';
import FormInput from '../atoms/FormInput';
import FormButton from '../atoms/FormButton';

/**
 * RegisterForm - Molecule that composes form inputs and button for registration
 */
function RegisterForm({ onSubmit, loading, serverError }) {
  const [fullName, setFullName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [errors, setErrors] = useState({});

  const validate = () => {
    const newErrors = {};
    if (!fullName.trim()) {
      newErrors.fullName = 'Full name is required.';
    }
    if (!email.trim()) {
      newErrors.email = 'Email is required.';
    } else if (!/\S+@\S+\.\S+/.test(email)) {
      newErrors.email = 'Please enter a valid email address.';
    }
    if (!password) {
      newErrors.password = 'Password is required.';
    } else if (password.length < 6) {
      newErrors.password = 'Password must be at least 6 characters.';
    }
    if (password !== confirmPassword) {
      newErrors.confirmPassword = 'Passwords do not match.';
    }
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    if (validate()) {
      onSubmit(email, password, confirmPassword, fullName);
    }
  };

  return (
    <form className="login-form" onSubmit={handleSubmit} noValidate>
      {serverError && (
        <div className="form-server-error">{serverError}</div>
      )}

      <FormInput
        id="register-fullname"
        label="Full Name"
        type="text"
        value={fullName}
        onChange={(e) => setFullName(e.target.value)}
        error={errors.fullName}
        placeholder="John Doe"
        required
        autoComplete="name"
      />

      <FormInput
        id="register-email"
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
        id="register-password"
        label="Password"
        type="password"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        error={errors.password}
        placeholder="At least 6 characters"
        required
        autoComplete="new-password"
      />

      <FormInput
        id="register-confirm"
        label="Confirm Password"
        type="password"
        value={confirmPassword}
        onChange={(e) => setConfirmPassword(e.target.value)}
        error={errors.confirmPassword}
        placeholder="Re-enter your password"
        required
        autoComplete="new-password"
      />

      <FormButton loading={loading}>Create Account</FormButton>
    </form>
  );
}

export default RegisterForm;
