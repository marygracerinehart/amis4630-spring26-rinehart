import {
  authReducer,
  initialAuthState,
  AUTH_ACTIONS,
  selectIsAuthenticated,
  selectIsAdmin,
} from './authReducer';

/**
 * Pure unit tests for the auth reducer.
 * No React, no DOM, no HTTP — just authReducer(state, action) and expect().
 */

// ─── Initial state ────────────────────────────────────────────────

test('initial state has null user, null token, loading true', () => {
  expect(initialAuthState).toEqual({
    user: null,
    token: null,
    loading: true,
  });
});

// ─── RESTORE_AUTH ─────────────────────────────────────────────────

test('RESTORE_AUTH sets user and token from payload', () => {
  const action = {
    type: AUTH_ACTIONS.RESTORE_AUTH,
    payload: {
      user: { userId: 'u1', email: 'a@b.com', roles: ['User'] },
      token: 'stored-jwt',
    },
  };

  const next = authReducer(initialAuthState, action);

  expect(next.user).toEqual({ userId: 'u1', email: 'a@b.com', roles: ['User'] });
  expect(next.token).toBe('stored-jwt');
  expect(next.loading).toBe(false);
});

test('RESTORE_AUTH with no stored data sets nulls and stops loading', () => {
  const action = { type: AUTH_ACTIONS.RESTORE_AUTH, payload: null };
  const next = authReducer(initialAuthState, action);

  expect(next.user).toBeNull();
  expect(next.token).toBeNull();
  expect(next.loading).toBe(false);
});

// ─── LOGIN_SUCCESS ────────────────────────────────────────────────

test('LOGIN_SUCCESS stores user and token', () => {
  const user = { userId: 'u2', email: 'login@test.com', fullName: 'Test', roles: ['User'] };
  const action = {
    type: AUTH_ACTIONS.LOGIN_SUCCESS,
    payload: { user, token: 'jwt-login' },
  };

  const next = authReducer(initialAuthState, action);

  expect(next.user).toEqual(user);
  expect(next.token).toBe('jwt-login');
  expect(next.loading).toBe(false);
});

test('LOGIN_SUCCESS overwrites previous user', () => {
  const prev = {
    user: { userId: 'old', email: 'old@test.com', roles: ['User'] },
    token: 'old-token',
    loading: false,
  };
  const newUser = { userId: 'new', email: 'new@test.com', roles: ['Admin'] };
  const action = {
    type: AUTH_ACTIONS.LOGIN_SUCCESS,
    payload: { user: newUser, token: 'new-token' },
  };

  const next = authReducer(prev, action);

  expect(next.user).toEqual(newUser);
  expect(next.token).toBe('new-token');
});

// ─── REGISTER_SUCCESS ─────────────────────────────────────────────

test('REGISTER_SUCCESS stores user and token', () => {
  const user = { userId: 'u3', email: 'reg@test.com', fullName: 'New User', roles: ['User'] };
  const action = {
    type: AUTH_ACTIONS.REGISTER_SUCCESS,
    payload: { user, token: 'jwt-register' },
  };

  const next = authReducer(initialAuthState, action);

  expect(next.user).toEqual(user);
  expect(next.token).toBe('jwt-register');
  expect(next.loading).toBe(false);
});

// ─── LOGOUT ───────────────────────────────────────────────────────

test('LOGOUT clears user and token', () => {
  const loggedIn = {
    user: { userId: 'u1', email: 'a@b.com', roles: ['User'] },
    token: 'some-jwt',
    loading: false,
  };

  const next = authReducer(loggedIn, { type: AUTH_ACTIONS.LOGOUT });

  expect(next.user).toBeNull();
  expect(next.token).toBeNull();
});

test('LOGOUT preserves loading flag', () => {
  const state = {
    user: { userId: 'u1' },
    token: 'jwt',
    loading: false,
  };

  const next = authReducer(state, { type: AUTH_ACTIONS.LOGOUT });

  expect(next.loading).toBe(false);
});

test('LOGOUT from already logged-out state is idempotent', () => {
  const empty = { user: null, token: null, loading: false };
  const next = authReducer(empty, { type: AUTH_ACTIONS.LOGOUT });

  expect(next.user).toBeNull();
  expect(next.token).toBeNull();
});

// ─── LOADING_COMPLETE ─────────────────────────────────────────────

test('LOADING_COMPLETE sets loading to false', () => {
  const next = authReducer(initialAuthState, { type: AUTH_ACTIONS.LOADING_COMPLETE });
  expect(next.loading).toBe(false);
});

test('LOADING_COMPLETE does not touch user or token', () => {
  const state = { user: { userId: 'u1' }, token: 'jwt', loading: true };
  const next = authReducer(state, { type: AUTH_ACTIONS.LOADING_COMPLETE });

  expect(next.user).toEqual({ userId: 'u1' });
  expect(next.token).toBe('jwt');
});

// ─── Unknown action ───────────────────────────────────────────────

test('unknown action returns state unchanged', () => {
  const state = { user: null, token: null, loading: false };
  const next = authReducer(state, { type: 'DOES_NOT_EXIST' });

  expect(next).toEqual(state);
});

// ─── Selectors ────────────────────────────────────────────────────

describe('selectIsAuthenticated', () => {
  test('returns true when token exists', () => {
    expect(selectIsAuthenticated({ token: 'abc' })).toBe(true);
  });

  test('returns false when token is null', () => {
    expect(selectIsAuthenticated({ token: null })).toBe(false);
  });

  test('returns false when token is empty string', () => {
    expect(selectIsAuthenticated({ token: '' })).toBe(false);
  });
});

describe('selectIsAdmin', () => {
  test('returns true when roles include Admin', () => {
    expect(selectIsAdmin({ user: { roles: ['Admin'] } })).toBe(true);
  });

  test('returns true when roles include Admin among others', () => {
    expect(selectIsAdmin({ user: { roles: ['User', 'Admin'] } })).toBe(true);
  });

  test('returns false for regular User role', () => {
    expect(selectIsAdmin({ user: { roles: ['User'] } })).toBe(false);
  });

  test('returns false when user is null', () => {
    expect(selectIsAdmin({ user: null })).toBe(false);
  });

  test('returns false when roles is undefined', () => {
    expect(selectIsAdmin({ user: {} })).toBe(false);
  });
});
