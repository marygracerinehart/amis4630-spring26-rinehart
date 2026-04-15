/**
 * Pure auth-state reducer — no React, no HTTP, no localStorage.
 *
 * Extracted from AuthContext so the state transitions can be tested
 * with plain objects: authReducer(state, action) → newState.
 */

export const initialAuthState = {
  user: null,
  token: null,
  loading: true,
};

/**
 * Action types — mirrors every state change that AuthContext performs.
 */
export const AUTH_ACTIONS = {
  RESTORE_AUTH: 'RESTORE_AUTH',
  LOGIN_SUCCESS: 'LOGIN_SUCCESS',
  REGISTER_SUCCESS: 'REGISTER_SUCCESS',
  LOGOUT: 'LOGOUT',
  LOADING_COMPLETE: 'LOADING_COMPLETE',
};

/**
 * Pure reducer — given current state and an action, returns new state.
 *
 * @param {Object} state   Current auth state
 * @param {Object} action  { type, payload? }
 * @returns {Object}       New auth state
 */
export function authReducer(state, action) {
  switch (action.type) {

    case AUTH_ACTIONS.RESTORE_AUTH:
      return {
        ...state,
        user: action.payload?.user ?? null,
        token: action.payload?.token ?? null,
        loading: false,
      };

    case AUTH_ACTIONS.LOGIN_SUCCESS:
    case AUTH_ACTIONS.REGISTER_SUCCESS:
      return {
        ...state,
        user: action.payload.user,
        token: action.payload.token,
        loading: false,
      };

    case AUTH_ACTIONS.LOGOUT:
      return {
        ...state,
        user: null,
        token: null,
      };

    case AUTH_ACTIONS.LOADING_COMPLETE:
      return {
        ...state,
        loading: false,
      };

    default:
      return state;
  }
}

// ── Derived selectors (pure functions) ────────────────────────────

export function selectIsAuthenticated(state) {
  return !!state.token;
}

export function selectIsAdmin(state) {
  return state.user?.roles?.includes('Admin') || false;
}
