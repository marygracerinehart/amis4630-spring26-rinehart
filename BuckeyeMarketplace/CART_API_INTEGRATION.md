# Cart API Integration Documentation

## Overview
The shopping cart fully replaces any localStorage-only approach with a complete backend API integration. All cart operations persist to the database and are managed by the CartController.

## Backend Cart API

### Base URL
```
http://localhost:5107/api/cart
```

### API Endpoints

#### 1. GET /api/cart
**Get or create cart for current user**

**Response (200 OK):**
```json
{
  "id": 1,
  "userId": "user-1",
  "items": [
    {
      "id": 101,
      "cartId": 1,
      "productId": 1,
      "title": "Introduction to Information Systems textbook",
      "price": 89.99,
      "quantity": 2,
      "imageUrl": "...",
      "category": "Textbooks",
      "sellerName": "John Smith"
    }
  ]
}
```

---

#### 2. POST /api/cart
**Add an item to cart**

**Request Body:**
```json
{
  "productId": 1,
  "quantity": 2
}
```

**Response (201 Created):**
```json
{
  "id": 101,
  "cartId": 1,
  "productId": 1,
  "title": "Introduction to Information Systems textbook",
  "price": 89.99,
  "quantity": 2,
  "imageUrl": "...",
  "category": "Textbooks",
  "sellerName": "John Smith"
}
```

**Error Cases:**
- `400 Bad Request` - Quantity <= 0
- `404 Not Found` - Product doesn't exist

---

#### 3. PUT /api/cart/{cartItemId}
**Update item quantity**

**Request Body:**
```json
{
  "quantity": 5
}
```

**Response (200 OK):**
```json
{
  "id": 101,
  "cartId": 1,
  "productId": 1,
  "title": "Introduction to Information Systems textbook",
  "price": 89.99,
  "quantity": 5,
  "imageUrl": "...",
  "category": "Textbooks",
  "sellerName": "John Smith"
}
```

**Error Cases:**
- `400 Bad Request` - Quantity <= 0
- `404 Not Found` - Cart item doesn't exist

---

#### 4. DELETE /api/cart/{cartItemId}
**Remove an item from cart**

**Response (200 OK):**
```json
{
  "message": "Item removed from cart."
}
```

**Error Cases:**
- `404 Not Found` - Cart item doesn't exist

---

#### 5. DELETE /api/cart/clear
**Clear entire cart**

**Response (200 OK):**
```json
{
  "message": "Cart cleared."
}
```

---

## Frontend Integration

### CartContext Hook (`src/context/CartContext.js`)

The `useCart()` hook provides full API integration:

```javascript
const { 
  // State
  items,           // Cart items array
  isLoading,       // Loading state
  error,           // Error message
  
  // Derived values
  itemCount,       // Total quantity of items
  subtotal,        // Sum of all item prices
  total,           // Total cost
  
  // API Actions
  addItem,         // async (product, quantity) => void
  removeItem,      // async (productId) => void
  updateQuantity,  // async (productId, quantity) => void
  clearCart,       // async () => void
} = useCart();
```

### Usage Examples

#### Add Item to Cart
```javascript
import { useCart } from '../context/CartContext';

function ProductCard({ product }) {
  const { addItem } = useCart();
  
  const handleAddToCart = () => {
    addItem(product, 1);  // Calls POST /api/cart
  };
  
  return <button onClick={handleAddToCart}>Add to Cart</button>;
}
```

#### Display Cart
```javascript
import { useCart } from '../context/CartContext';

function Cart() {
  const { items, itemCount, total } = useCart();
  
  return (
    <div>
      <h1>Shopping Cart ({itemCount})</h1>
      <p>Total: ${total.toFixed(2)}</p>
    </div>
  );
}
```

#### Update Quantity
```javascript
const { updateQuantity } = useCart();

updateQuantity(productId, 5);  // Calls PUT /api/cart/{itemId}
```

#### Remove Item
```javascript
const { removeItem } = useCart();

removeItem(productId);  // Calls DELETE /api/cart/{itemId}
```

#### Clear Cart
```javascript
const { clearCart } = useCart();

clearCart();  // Calls DELETE /api/cart/clear
```

---

## Data Flow

### Add to Cart Flow
```
1. User clicks "Add to Cart" in ProductCard
   ↓
2. addItem() is called with product and quantity
   ↓
3. POST /api/cart with { productId, quantity }
   ↓
4. Backend adds item to database and returns CartItem
   ↓
5. Frontend fetches latest cart with GET /api/cart
   ↓
6. CartContext updates state with new items
   ↓
7. Components re-render with updated cart
```

### Update Quantity Flow
```
1. User changes quantity in Cart page
   ↓
2. updateQuantity(productId, newQuantity) is called
   ↓
3. PUT /api/cart/{cartItemId} with { quantity: newQuantity }
   ↓
4. Backend updates database
   ↓
5. Frontend refreshes cart from GET /api/cart
   ↓
6. Components re-render with new quantity
```

---

## Backend Implementation Details

### CartController (`backend/BuckeyeMarketplaceAPI/Controllers/CartController.cs`)

**Key Features:**
- Uses hardcoded userId "user-1" (to be replaced with auth in M5)
- All cart data persisted to `AppDbContext.Carts` and `AppDbContext.CartItems`
- Validates product exists before adding to cart
- Automatically populates product info (title, price, image, seller) from Products table
- Validates quantity > 0 for all operations

### Models
- **Cart** - Container for cart items, linked to user
- **CartItem** - Individual item in cart with product details and quantity
- **Product** - Referenced for lookup and price/image data

---

## Database Schema

### Carts Table
| Column | Type | Notes |
|--------|------|-------|
| Id | int (PK) | Auto-increment |
| UserId | string | Currently hardcoded to "user-1" |

### CartItems Table
| Column | Type | Notes |
|--------|------|-------|
| Id | int (PK) | Auto-increment |
| CartId | int (FK) | References Carts.Id |
| ProductId | int | References Products.Id |
| Quantity | int | Validated > 0 |
| Title | string | Denormalized from Product |
| Price | decimal | Denormalized from Product |
| ImageUrl | string | Denormalized from Product |
| Category | string | Denormalized from Product |
| SellerName | string | Denormalized from Product |

---

## Test Coverage

### Backend Tests (`backend/BuckeyeMarketplaceAPI.Tests/CartControllerTests.cs`)

✅ **GetCart_Returns200** - GET /api/cart returns cart
✅ **AddToCart_ValidProduct_Returns201** - POST /api/cart with valid product
✅ **AddToCart_InvalidProductId_Returns404** - POST /api/cart with invalid product
✅ **AddToCart_ZeroQuantity_Returns400** - POST /api/cart with quantity 0
✅ **UpdateCartItem_Returns200WithUpdatedQuantity** - PUT /api/cart/{id}
✅ **RemoveCartItem_Returns200** - DELETE /api/cart/{id}
✅ **ClearCart_Returns200** - DELETE /api/cart/clear

**Test Status:** ✅ All 7 tests passing

---

## Configuration

### Environment Variables
```
# frontend/.env
REACT_APP_API_URL=http://localhost:5107
```

### API URL Resolution
```javascript
// CartContext.js
const API_BASE_URL = `${process.env.REACT_APP_API_URL || 'http://localhost:5107'}/api/cart`;
```

---

## No localStorage Used

✅ **Verified:** No localStorage or sessionStorage usage in frontend
- All cart data flows through API
- State managed in CartContext reducer
- Database persists cart between sessions
- User "user-1" cart restored on app load

---

## Next Steps (Milestone 5)

- Replace hardcoded userId with authenticated user from Azure AD
- Implement proper authorization for user carts
- Add cart persistence across device/browser sessions
- Consider cart expiration policies
