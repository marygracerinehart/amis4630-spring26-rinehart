# Lab Evaluation Report

**Student Repository**: `marygracerinehart-amis4630-spring26-rinehart`  
**Date**: May 6, 2026  
**Rubric**: rubric.md (Milestone 4 — Cart Feature, 25 points)

## 0. Build & Run Status

| Component           | Build | Runs | Notes                                                                                                                                                                                     |
| ------------------- | ----- | ---- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Backend (.NET)      | ✅    | ❌   | `dotnet build` succeeded. Runtime fails — requires Azure SQL connection string via `dotnet user-secrets` (not available locally)                                                          |
| Frontend (React/TS) | ✅    | ✅   | `npm run build` succeeded (1 eslint warning: missing dep in useCallback). Dev server running on localhost:3000                                                                            |
| API Endpoints       | —     | ❌   | Backend cannot start without DB connection string; endpoints not verifiable locally                                                                                                       |
| Backend Tests       | —     | ⚠️   | 48 passed, 51 failed. Failures are all integration tests requiring DB connection (WebApplicationFactory). Pure unit tests (PasswordRuleValidator, CartToOrderMapper, OrderTotal) all pass |
| Frontend Tests      | —     | ✅   | 45 tests passed across 3 suites. 1 suite failed (App.test.js — stale CRA test cannot resolve react-router-dom)                                                                            |

## 1. Project Structure

| Area                  | Expected                                  | Found                                                 | Status |
| --------------------- | ----------------------------------------- | ----------------------------------------------------- | ------ |
| Cart Model            | Backend model for Cart entity             | `Models/Cart.cs`                                      | ✅     |
| CartItem Model        | Backend model for CartItem entity         | `Models/CartItem.cs`                                  | ✅     |
| Cart Controller       | API controller for cart endpoints         | `Controllers/CartController.cs`                       | ✅     |
| Cart Context (FE)     | React context for cart state              | `src/context/CartContext.js`                          | ✅     |
| Cart Service (FE)     | Service layer for cart API calls          | `src/services/cartService.js`                         | ✅     |
| Cart Page (FE)        | Cart page component                       | `src/pages/Cart.js`                                   | ✅     |
| CartItem Component    | Molecule for individual cart item         | `src/components/molecules/CartItem.js`                | ✅     |
| CartPage Organism     | Organism composing cart layout            | `src/components/organisms/CartPage.js`                | ✅     |
| CartContent / Summary | Order summary component                   | `src/components/organisms/CartContent.js`             | ✅     |
| EF Migration          | Migration creating Carts/CartItems tables | `Migrations/20260501153414_InitialCreateSqlServer.cs` | ✅     |
| AI Usage Doc          | Milestone 4 AI usage documentation        | `docs/M4_AI-Usage`                                    | ✅     |

## 2. Rubric Scorecard

| #   | Requirement                              | Points | Status | Evidence                                                                                                                                                                                                                                                                                                                                                                                                        |
| --- | ---------------------------------------- | ------ | ------ | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 1a  | useReducer or Context API for cart state | 2      | ✅ Met | [CartContext.js](BuckeyeMarketplace/frontend/src/context/CartContext.js#L1-L4) — `useReducer` with `CartContext` via `createContext`; reducer handles SET_CART, ADD_ITEM, REMOVE_ITEM, UPDATE_QUANTITY, CLEAR_CART, SET_LOADING, SET_ERROR                                                                                                                                                                      |
| 1b  | Add, update quantity, remove operations  | 2      | ✅ Met | [CartContext.js](BuckeyeMarketplace/frontend/src/context/CartContext.js#L140-L195) — `addItem`, `updateQuantity`, `removeItem`, `clearCart` action creators all dispatch to reducer and call API                                                                                                                                                                                                                |
| 1c  | Cart count in header + calculated totals | 1      | ✅ Met | [Header.js](BuckeyeMarketplace/frontend/src/components/organisms/Header.js#L55-L57) — `itemCount` from `useCart()` displayed as badge; [CartContext.js](BuckeyeMarketplace/frontend/src/context/CartContext.js#L125-L133) — `itemCount`, `subtotal`, `total` computed via `useMemo`                                                                                                                             |
| 2a  | GET /api/cart                            | 1      | ✅ Met | [CartController.cs](BuckeyeMarketplace/backend/BuckeyeMarketplaceAPI/Controllers/CartController.cs#L66-L72) — `[HttpGet]` returns `Ok(cart)`                                                                                                                                                                                                                                                                    |
| 2b  | POST /api/cart (add item)                | 1      | ✅ Met | [CartController.cs](BuckeyeMarketplace/backend/BuckeyeMarketplaceAPI/Controllers/CartController.cs#L78-L127) — `[HttpPost]` validates product/stock, handles existing items, returns `CreatedAtAction`                                                                                                                                                                                                          |
| 2c  | PUT /api/cart/{cartItemId} (update qty)  | 1      | ✅ Met | [CartController.cs](BuckeyeMarketplace/backend/BuckeyeMarketplaceAPI/Controllers/CartController.cs#L133-L166) — `[HttpPut("{cartItemId}")]` validates quantity and stock, returns `Ok(item)`                                                                                                                                                                                                                    |
| 2d  | DELETE endpoints (item + clear)          | 1      | ✅ Met | [CartController.cs](BuckeyeMarketplace/backend/BuckeyeMarketplaceAPI/Controllers/CartController.cs#L171-L204) — `[HttpDelete("{cartItemId}")]` removes single item; `[HttpDelete("clear")]` removes all items                                                                                                                                                                                                   |
| 2e  | Proper status codes and responses        | 1      | ✅ Met | Controller uses `Ok`, `CreatedAtAction`, `BadRequest`, `NotFound` with descriptive message objects throughout                                                                                                                                                                                                                                                                                                   |
| 3a  | Cart/CartItem EF entities                | 2      | ✅ Met | [Cart.cs](BuckeyeMarketplace/backend/BuckeyeMarketplaceAPI/Models/Cart.cs) — `Cart` with `Id`, `UserId`, `Items` collection; [CartItem.cs](BuckeyeMarketplace/backend/BuckeyeMarketplaceAPI/Models/CartItem.cs) — `CartItem` with `Id`, `CartId`, `ProductId`, `Quantity`, denormalized product fields                                                                                                          |
| 3b  | Relationships and navigation properties  | 1      | ✅ Met | [AppDbContext.cs](BuckeyeMarketplace/backend/BuckeyeMarketplaceAPI/Data/AppDbContext.cs#L37-L52) — Cart→CartItems one-to-many with cascade delete; CartItem→Product many-to-one with restrict delete; navigation properties on both entities                                                                                                                                                                    |
| 3c  | Migrations applied, data persists        | 1      | ✅ Met | [20260501153414_InitialCreateSqlServer.cs](BuckeyeMarketplace/backend/BuckeyeMarketplaceAPI/Migrations/20260501153414_InitialCreateSqlServer.cs) — Creates `Carts` and `CartItems` tables with FKs and indexes; `AppDbContextModelSnapshot.cs` present                                                                                                                                                          |
| 4a  | Real API replaces mock/localStorage      | 2      | ✅ Met | [cartService.js](BuckeyeMarketplace/frontend/src/services/cartService.js) — All operations use `apiFetch` to call `/api/cart` endpoints; no localStorage or mock data found                                                                                                                                                                                                                                     |
| 4b  | All cart operations call API             | 2      | ✅ Met | [CartContext.js](BuckeyeMarketplace/frontend/src/context/CartContext.js#L140-L210) — `addItem` calls `cartService.addItemToCart`, `removeItem` calls `cartService.removeItemFromCart`, `updateQuantity` calls `cartService.updateCartItemQuantity`, `clearCart` calls `cartService.clearEntireCart`                                                                                                             |
| 4c  | State synchronization                    | 1      | ✅ Met | [CartContext.js](BuckeyeMarketplace/frontend/src/context/CartContext.js#L95-L113) — `useEffect` fetches cart on mount; each mutation re-fetches full cart from server (`fetchCart()` after add/remove/update) to keep state in sync                                                                                                                                                                             |
| 5a  | Loading states                           | 1      | ✅ Met | [CartContext.js](BuckeyeMarketplace/frontend/src/context/CartContext.js#L97) — `SET_LOADING` dispatched; [Cart.js](BuckeyeMarketplace/frontend/src/pages/Cart.js#L45-L55) — renders "Loading your cart..." when `isLoading` is true; [ProductDetail.js](BuckeyeMarketplace/frontend/src/pages/ProductDetail.js#L40) — "Loading product..." state                                                                |
| 5b  | Error messages and edge cases            | 1      | ✅ Met | [Cart.js](BuckeyeMarketplace/frontend/src/pages/Cart.js#L57-L67) — error state renders red error message; [CartController.cs](BuckeyeMarketplace/backend/BuckeyeMarketplaceAPI/Controllers/CartController.cs#L81-L100) — validates zero/negative quantity, missing product, out-of-stock, exceeds stock                                                                                                         |
| 5c  | Success feedback                         | 1      | ✅ Met | [ProductDetail.js](BuckeyeMarketplace/frontend/src/pages/ProductDetail.js#L39) — `addNotification` with "added to cart!" success toast; [CartItem.js](BuckeyeMarketplace/frontend/src/components/molecules/CartItem.js#L9-L12) — removal shows "removed from cart" notification; [Toast.js](BuckeyeMarketplace/frontend/src/components/organisms/Toast.js) — toast notification component renders notifications |
| 6a  | Clean component structure                | 1      | ✅ Met | Atomic design hierarchy: atoms (`AddToCartButton`, `CartActionButton`, `PriceDisplay`), molecules (`CartItem`), organisms (`CartPage`, `CartContent`, `Header`), pages (`Cart.js`)                                                                                                                                                                                                                              |
| 6b  | Service layer / custom hooks             | 1      | ✅ Met | [cartService.js](BuckeyeMarketplace/frontend/src/services/cartService.js) — dedicated service with `fetchCart`, `addItemToCart`, `removeItemFromCart`, `updateCartItemQuantity`, `clearEntireCart`; [CartContext.js](BuckeyeMarketplace/frontend/src/context/CartContext.js#L118) — custom `useCart` hook                                                                                                       |
| 6c  | AI usage documented                      | 1      | ✅ Met | [M4_AI-Usage](docs/M4_AI-Usage) — documents tools (Claude, GitHub Copilot), prompts used, and what was done manually                                                                                                                                                                                                                                                                                            |

**Total: 25 / 25**

## 3. Detailed Findings

All rubric items are met. No deficiencies to report.

## 4. Action Plan

No corrective actions required — full marks earned.

## 5. Code Quality Coaching (Non-Scoring)

- **Notification timer leak**: [NotificationContext.js](BuckeyeMarketplace/frontend/src/context/NotificationContext.js#L14-L17) — `setTimeout` inside `addNotification` references `removeNotification` but `removeNotification` is not in the dependency array of `useCallback`. Also, there is no cleanup if the component unmounts before the timeout fires. Consider returning a cleanup function or using `useEffect` for auto-dismiss.

- **Missing file extension on AI doc**: [M4_AI-Usage](docs/M4_AI-Usage) — the file has no `.md` extension. While content is fine, using `M4_AI-Usage.md` would be more conventional and render properly in GitHub/VS Code previews.

- **Stock validation race condition**: [CartController.cs](BuckeyeMarketplace/backend/BuckeyeMarketplaceAPI/Controllers/CartController.cs#L95-L100) — stock is checked before the add/update but there is no concurrency guard (e.g., optimistic concurrency token on `StockQuantity`). Under concurrent requests, two users could both pass the stock check. For a course project this is fine, but worth noting for production systems.

- **Error messages could be more specific on frontend**: [cartService.js](BuckeyeMarketplace/frontend/src/services/cartService.js#L28) — when the API returns a structured error (e.g., "out of stock" message), the service throws a generic "Failed to add item to cart" instead of forwarding the server's message body. Parsing `response.json()` on error responses and surfacing the server message would improve UX.

## 6. Git Practices Coaching (Non-Scoring)

- **Commit granularity**: The migration file `20260501153414_InitialCreateSqlServer` consolidates all tables (including Cart/CartItem) into a single initial migration. In a real project, consider incremental migrations for each feature so the git history tells a story of how the schema evolved.

- **Documentation file naming**: The AI usage document at `docs/M4_AI-Usage` lacks a file extension, which can cause rendering issues in some tools. Using `.md` consistently helps maintain a professional repository.

---

**25/25** — Excellent work. All cart feature requirements are fully implemented with a well-structured frontend (useReducer + Context, service layer, atomic design), complete CRUD API endpoints with proper status codes, EF Core persistence with relationships and migrations, and real frontend-backend integration with state synchronization. The coaching notes above (notification cleanup, error message forwarding, stock concurrency, file naming) are suggestions for professional growth, not scoring deductions.
