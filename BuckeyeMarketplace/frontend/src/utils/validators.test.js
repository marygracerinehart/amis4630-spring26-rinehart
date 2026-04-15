import { validateEmail } from './validators';

/**
 * Pure unit tests for the validateEmail helper.
 * No React rendering, no HTTP, no DOM — just import + assert.
 */

// ─── Valid emails ──────────────────────────────────────────────────

test('accepts a standard email', () => {
  expect(validateEmail('user@example.com')).toEqual({ valid: true, error: null });
});

test('accepts email with subdomain', () => {
  expect(validateEmail('user@mail.example.com')).toEqual({ valid: true, error: null });
});

test('accepts email with plus addressing', () => {
  expect(validateEmail('user+tag@example.com')).toEqual({ valid: true, error: null });
});

test('accepts email with dots in local part', () => {
  expect(validateEmail('first.last@example.com')).toEqual({ valid: true, error: null });
});

test('accepts email with numbers', () => {
  expect(validateEmail('user123@example456.com')).toEqual({ valid: true, error: null });
});

// ─── Missing / empty ──────────────────────────────────────────────

test('rejects null', () => {
  const result = validateEmail(null);
  expect(result.valid).toBe(false);
  expect(result.error).toMatch(/required/i);
});

test('rejects undefined', () => {
  const result = validateEmail(undefined);
  expect(result.valid).toBe(false);
  expect(result.error).toMatch(/required/i);
});

test('rejects empty string', () => {
  const result = validateEmail('');
  expect(result.valid).toBe(false);
  expect(result.error).toMatch(/required/i);
});

test('rejects whitespace-only string', () => {
  const result = validateEmail('   ');
  expect(result.valid).toBe(false);
  expect(result.error).toMatch(/required/i);
});

// ─── Invalid formats ──────────────────────────────────────────────

test('rejects plain text without @', () => {
  const result = validateEmail('notanemail');
  expect(result.valid).toBe(false);
  expect(result.error).toMatch(/valid email/i);
});

test('rejects missing domain', () => {
  const result = validateEmail('user@');
  expect(result.valid).toBe(false);
  expect(result.error).toMatch(/valid email/i);
});

test('rejects missing local part', () => {
  const result = validateEmail('@example.com');
  expect(result.valid).toBe(false);
  expect(result.error).toMatch(/valid email/i);
});

test('rejects missing TLD', () => {
  const result = validateEmail('user@example');
  expect(result.valid).toBe(false);
  expect(result.error).toMatch(/valid email/i);
});

test('accepts double @ (matches existing form regex)', () => {
  // The /\S+@\S+\.\S+/ pattern used in LoginForm & RegisterForm
  // is intentionally loose — it does not reject double @.
  expect(validateEmail('user@@example.com')).toEqual({ valid: true, error: null });
});

// ─── Return shape ─────────────────────────────────────────────────

test('valid result has error: null', () => {
  expect(validateEmail('a@b.c').error).toBeNull();
});

test('invalid result has valid: false', () => {
  expect(validateEmail('bad').valid).toBe(false);
});
