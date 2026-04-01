import React, { createContext, useReducer, useContext, useMemo, useEffect } from 'react';
import * as cartService from '../services/cartService';

const CartContext = createContext();

// Action types
export const CART_ACTIONS = {
  ADD_ITEM: 'ADD_ITEM',
  REMOVE_ITEM: 'REMOVE_ITEM',
  UPDATE_QUANTITY: 'UPDATE_QUANTITY',
  CLEAR_CART: 'CLEAR_CART',
  SET_CART: 'SET_CART',
  SET_LOADING: 'SET_LOADING',
  SET_ERROR: 'SET_ERROR',
};

// Reducer function
const cartReducer = (state, action) => {
  switch (action.type) {
    case CART_ACTIONS.SET_CART:
      return {
        ...state,
        items: action.payload,
      };

    case CART_ACTIONS.ADD_ITEM: {
      const existingItem = state.items.find((item) => item.id === action.payload.id);

      if (existingItem) {
        return {
          ...state,
          items: state.items.map((item) =>
            item.id === action.payload.id
              ? { ...item, quantity: item.quantity + (action.payload.quantity || 1) }
              : item
          ),
        };
      }

      return {
        ...state,
        items: [...state.items, { ...action.payload, quantity: action.payload.quantity || 1 }],
      };
    }

    case CART_ACTIONS.REMOVE_ITEM:
      return {
        ...state,
        items: state.items.filter((item) => item.id !== action.payload),
      };

    case CART_ACTIONS.UPDATE_QUANTITY:
      return {
        ...state,
        items: state.items.map((item) =>
          item.id === action.payload.id
            ? { ...item, quantity: Math.max(1, action.payload.quantity) }
            : item
        ),
      };

    case CART_ACTIONS.CLEAR_CART:
      return {
        items: [],
      };

    case CART_ACTIONS.SET_LOADING:
      return {
        ...state,
        isLoading: action.payload,
      };

    case CART_ACTIONS.SET_ERROR:
      return {
        ...state,
        error: action.payload,
      };

    default:
      return state;
  }
};

// CartProvider component
export function CartProvider({ children }) {
  const [state, dispatch] = useReducer(cartReducer, { 
    items: [],
    isLoading: true,
    error: null,
  });

  // Fetch cart data from backend on mount
  useEffect(() => {
    const fetchCart = async () => {
      try {
        dispatch({ type: CART_ACTIONS.SET_LOADING, payload: true });
        const items = await cartService.fetchCart();
        dispatch({
          type: CART_ACTIONS.SET_CART,
          payload: items,
        });
        dispatch({ type: CART_ACTIONS.SET_ERROR, payload: null });
      } catch (error) {
        console.error('Error fetching cart:', error);
        dispatch({ type: CART_ACTIONS.SET_ERROR, payload: error.message });
      } finally {
        dispatch({ type: CART_ACTIONS.SET_LOADING, payload: false });
      }
    };

    fetchCart();
  }, []);

  return (
    <CartContext.Provider value={{ state, dispatch }}>
      {children}
    </CartContext.Provider>
  );
}

// Custom useCart hook
export function useCart() {
  const context = useContext(CartContext);

  if (!context) {
    throw new Error('useCart must be used within CartProvider');
  }

  const { state, dispatch } = context;

  // Derived values
  const itemCount = useMemo(
    () => state.items.reduce((count, item) => count + item.quantity, 0),
    [state.items]
  );

  const subtotal = useMemo(
    () => state.items.reduce((total, item) => total + item.price * item.quantity, 0),
    [state.items]
  );

  const total = useMemo(() => subtotal, [subtotal]);

  // Action creators with API calls
  const addItem = async (product, quantity = 1) => {
    try {
      dispatch({ type: CART_ACTIONS.SET_ERROR, payload: null });
      const items = await cartService.addItemToCart(product.id, quantity);
      dispatch({
        type: CART_ACTIONS.SET_CART,
        payload: items,
      });
    } catch (error) {
      console.error('Error adding item:', error);
      dispatch({ type: CART_ACTIONS.SET_ERROR, payload: error.message });
    }
  };

  const removeItem = async (productId) => {
    try {
      dispatch({ type: CART_ACTIONS.SET_ERROR, payload: null });
      
      // Find the cartItemId and product title from the current state
      const cartItem = state.items.find(item => item.productId === productId);
      if (!cartItem) {
        console.warn('Item not found in cart');
        return null;
      }

      const items = await cartService.removeItemFromCart(cartItem.id);
      dispatch({
        type: CART_ACTIONS.SET_CART,
        payload: items,
      });
      
      // Return product title for success notification
      return cartItem.title;
    } catch (error) {
      console.error('Error removing item:', error);
      dispatch({ type: CART_ACTIONS.SET_ERROR, payload: error.message });
      return null;
    }
  };

  const updateQuantity = async (productId, quantity) => {
    try {
      dispatch({ type: CART_ACTIONS.SET_ERROR, payload: null });
      
      // Find the cartItemId from the current state
      const cartItem = state.items.find(item => item.productId === productId);
      if (!cartItem) {
        console.warn('Item not found in cart');
        return;
      }

      const items = await cartService.updateCartItemQuantity(cartItem.id, quantity);
      dispatch({
        type: CART_ACTIONS.SET_CART,
        payload: items,
      });
    } catch (error) {
      console.error('Error updating quantity:', error);
      dispatch({ type: CART_ACTIONS.SET_ERROR, payload: error.message });
    }
  };

  const clearCart = async () => {
    try {
      dispatch({ type: CART_ACTIONS.SET_ERROR, payload: null });
      await cartService.clearEntireCart();
      dispatch({
        type: CART_ACTIONS.SET_CART,
        payload: [],
      });
    } catch (error) {
      console.error('Error clearing cart:', error);
      dispatch({ type: CART_ACTIONS.SET_ERROR, payload: error.message });
    }
  };

  return {
    // State
    items: state.items,
    isLoading: state.isLoading,
    error: state.error,
    // Derived values
    itemCount,
    subtotal,
    total,
    // Actions
    addItem,
    removeItem,
    updateQuantity,
    clearCart,
    dispatch,
  };
}
