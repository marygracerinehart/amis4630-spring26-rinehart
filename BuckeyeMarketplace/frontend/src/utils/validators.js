/**
 * Form-validation helpers — pure functions, no React, no HTTP.
 *
 * The email rule mirrors the regex already used in
 * LoginForm.js and RegisterForm.js: /\S+@\S+\.\S+/
 */

/**
 * Validates an email address.
 * @param {string} email
 * @returns {{ valid: boolean, error: string | null }}
 */
export function validateEmail(email) {
  if (email === undefined || email === null || email.trim() === '') {
    return { valid: false, error: 'Email is required.' };
  }

  // Same pattern used in LoginForm / RegisterForm
  if (!/\S+@\S+\.\S+/.test(email)) {
    return { valid: false, error: 'Please enter a valid email address.' };
  }

  return { valid: true, error: null };
}
