import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';
import '@testing-library/jest-dom';
import LoginForm from './LoginForm';

/**
 * Rendering + validation tests for <LoginForm />.
 * Asserts that error messages appear when the form is submitted with empty fields.
 */

describe('LoginForm', () => {
  const noop = jest.fn();

  beforeEach(() => {
    noop.mockClear();
  });

  // ─── Renders correctly ────────────────────────────────────────────

  test('renders email input, password input, and submit button', () => {
    render(<LoginForm onSubmit={noop} />);

    expect(screen.getByLabelText(/email/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/password/i)).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /sign in/i })).toBeInTheDocument();
  });

  // ─── Empty-field validation ───────────────────────────────────────

  test('shows email error when submitted with empty email', () => {
    render(<LoginForm onSubmit={noop} />);

    fireEvent.click(screen.getByRole('button', { name: /sign in/i }));

    expect(screen.getByText('Email is required.')).toBeInTheDocument();
  });

  test('shows password error when submitted with empty password', () => {
    render(<LoginForm onSubmit={noop} />);

    fireEvent.click(screen.getByRole('button', { name: /sign in/i }));

    expect(screen.getByText('Password is required.')).toBeInTheDocument();
  });

  test('shows both errors when submitted with all fields empty', () => {
    render(<LoginForm onSubmit={noop} />);

    fireEvent.click(screen.getByRole('button', { name: /sign in/i }));

    expect(screen.getByText('Email is required.')).toBeInTheDocument();
    expect(screen.getByText('Password is required.')).toBeInTheDocument();
  });

  test('does NOT call onSubmit when fields are empty', () => {
    render(<LoginForm onSubmit={noop} />);

    fireEvent.click(screen.getByRole('button', { name: /sign in/i }));

    expect(noop).not.toHaveBeenCalled();
  });

  // ─── Invalid email format ────────────────────────────────────────

  test('shows email format error for invalid email', () => {
    render(<LoginForm onSubmit={noop} />);

    fireEvent.change(screen.getByLabelText(/email/i), {
      target: { value: 'not-an-email' },
    });
    fireEvent.click(screen.getByRole('button', { name: /sign in/i }));

    expect(screen.getByText('Please enter a valid email address.')).toBeInTheDocument();
  });

  // ─── Valid submission ─────────────────────────────────────────────

  test('calls onSubmit with email and password when valid', () => {
    render(<LoginForm onSubmit={noop} />);

    fireEvent.change(screen.getByLabelText(/email/i), {
      target: { value: 'user@example.com' },
    });
    fireEvent.change(screen.getByLabelText(/password/i), {
      target: { value: 'MyPassword1' },
    });
    fireEvent.click(screen.getByRole('button', { name: /sign in/i }));

    expect(noop).toHaveBeenCalledWith('user@example.com', 'MyPassword1');
  });

  test('no error messages shown after valid submission', () => {
    render(<LoginForm onSubmit={noop} />);

    fireEvent.change(screen.getByLabelText(/email/i), {
      target: { value: 'user@example.com' },
    });
    fireEvent.change(screen.getByLabelText(/password/i), {
      target: { value: 'MyPassword1' },
    });
    fireEvent.click(screen.getByRole('button', { name: /sign in/i }));

    expect(screen.queryByText('Email is required.')).not.toBeInTheDocument();
    expect(screen.queryByText('Password is required.')).not.toBeInTheDocument();
  });

  // ─── Server error display ────────────────────────────────────────

  test('displays server error when passed as prop', () => {
    render(<LoginForm onSubmit={noop} serverError="Invalid credentials." />);

    expect(screen.getByText('Invalid credentials.')).toBeInTheDocument();
  });
});
