# Lab Evaluation Report

**Student Repository**: `marygracerinehart-amis4630-spring26-rinehart`  
**Date**: 2026-03-23  
**Rubric**: `rubric.md`

## 1. Build & Run Status

| Component           | Build | Runs | Notes                                                              |
| ------------------- | ----- | ---- | ------------------------------------------------------------------ |
| Backend (.NET)      | ✅    | ✅   | `dotnet build` succeeded. Server runs on `http://localhost:5107`.  |
| Frontend (React/TS) | ✅    | ✅   | `npm install` and `npm run build` both succeeded (CRA project).    |
| API Endpoints       | —     | ✅   | All endpoints tested and responding correctly (see details below). |

**API Endpoint Verification:**

| Endpoint                | HTTP Status | Result                                                                 |
| ----------------------- | ----------- | ---------------------------------------------------------------------- |
| `GET /api/products`     | 200         | Returns JSON array of 9 products with correct shape                    |
| `GET /api/products/1`   | 200         | Returns single product: "Introduction to Information Systems textbook" |
| `GET /api/products/999` | 404         | Correctly returns 404 for unknown ID                                   |

### Project Structure Comparison

| Expected    | Found                           | Status |
| ----------- | ------------------------------- | ------ |
| `/backend`  | `/BuckeyeMarketplace/backend/`  | ✅     |
| `/frontend` | `/BuckeyeMarketplace/frontend/` | ✅     |
| `/docs`     | `/docs/`                        | ✅     |

> Note: `backend/` and `frontend/` are nested one level deep under `BuckeyeMarketplace/` rather than at the repo root, but the solution file structure is present and correctly organized.

## 2. Rubric Scorecard

| #   | Requirement                          | Points | Status | Evidence                                                                                                                                                                                                                                                                                                                                                                              |
| --- | ------------------------------------ | ------ | ------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 1   | React Product List Page              | 4.5    | ✅ Met | [ProductList.js](BuckeyeMarketplace/frontend/src/pages/ProductList.js) — Displays all fields (title, category, sellerName, price, image); loading state (L33), empty state (L51), and error state (L37). Component hierarchy does not follow Atomic Design from M2.                                                                                                                   |
| 2   | React Product Detail Page            | 5      | ✅ Met | [ProductDetail.js](BuckeyeMarketplace/frontend/src/pages/ProductDetail.js) — Separate route at `/product/:id` ([App.js L10](BuckeyeMarketplace/frontend/src/App.js#L10)); all fields displayed (title, category, price, description, sellerName, postedDate, imageUrl); bidirectional navigation via Link from list and "Back to Products" button.                                    |
| 3   | API Endpoint: GET /api/products      | 5      | ✅ Met | [ProductsController.cs](BuckeyeMarketplace/backend/BuckeyeMarketplaceAPI/Controllers/ProductsController.cs#L18-L22) — Returns `Ok(products)` (200 status) with correct JSON array; in-memory `static List<Product>` with 9 seed products.                                                                                                                                             |
| 4   | API Endpoint: GET /api/products/{id} | 5      | ✅ Met | [ProductsController.cs](BuckeyeMarketplace/backend/BuckeyeMarketplaceAPI/Controllers/ProductsController.cs#L24-L30) — Returns single product by ID with `Ok(product)`; returns `NotFound()` (404) for unknown IDs. Verified via live test.                                                                                                                                            |
| 5   | Frontend-to-API Integration          | 5      | ✅ Met | [ProductList.js L19](BuckeyeMarketplace/frontend/src/pages/ProductList.js#L19) fetches from `http://localhost:5107/api/products`; [ProductDetail.js L20](BuckeyeMarketplace/frontend/src/pages/ProductDetail.js#L20) fetches from `http://localhost:5107/api/products/${id}`. No hardcoded data in components. Error state handled in both components with try/catch and UI feedback. |

**Total: 24.5 / 25**

## 3. Detailed Findings

### Item #1: React Product List Page

**What was expected**: Component hierarchy follows Atomic Design from M2 (for full Excellent marks).  
**What was found**: The ProductList page is implemented as a single monolithic component. The M2 Atomic Design hierarchy document ([AtomicDesignComponent Hiearchy.txt](docs/AtomicDesignComponent%20Hiearchy.txt)) defines atoms (Button, ProductImage, PriceText, etc.), molecules (ProductInfo, SearchBar), and organisms (ProductCard, ProductGrid) — none of which are implemented as separate reusable components.  
**Gap**: The product card, product image, price display, and other elements are rendered inline within ProductList rather than being broken into reusable Atomic Design components (atoms → molecules → organisms). All functional requirements (loading, empty, error states, all fields) are fully met.

## 4. Action Plan

1. **[0.5pts] React Product List Page — Atomic Design**: Refactor [ProductList.js](BuckeyeMarketplace/frontend/src/pages/ProductList.js) to extract reusable components per the Atomic Design hierarchy defined in M2. Create `atoms/` (e.g., `ProductImage`, `PriceText`, `Button`), `molecules/` (e.g., `ProductInfo`), and `organisms/` (e.g., `ProductCard`) component directories and import them into the list page.

## 5. Code Quality Coaching (Non-Scoring)

- **Hardcoded API URL**: [ProductList.js L19](BuckeyeMarketplace/frontend/src/pages/ProductList.js#L19) and [ProductDetail.js L20](BuckeyeMarketplace/frontend/src/pages/ProductDetail.js#L20) hardcode `http://localhost:5107`. Extract this into an environment variable (e.g., `REACT_APP_API_URL`) so it can be configured for different environments (development, staging, production).

- **Base64-encoded images in controller**: [ProductsController.cs](BuckeyeMarketplace/backend/BuckeyeMarketplaceAPI/Controllers/ProductsController.cs) — Several product `ImageUrl` values contain large base64-encoded data URIs (products 2, 3, 5, 6, 7, 8, 9). This bloats the API response significantly. Use hosted image URLs or serve images as static files instead.

- **Overly permissive CORS policy**: [Program.cs L6-L10](BuckeyeMarketplace/backend/BuckeyeMarketplaceAPI/Program.cs#L6-L10) uses `AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()`. This is acceptable for local development but should be restricted to specific origins in production to prevent unauthorized cross-origin requests.

- **React useEffect dependency warning**: [ProductDetail.js L16](BuckeyeMarketplace/frontend/src/pages/ProductDetail.js#L16) — The `fetchProduct` function is called inside `useEffect` but is defined outside it, which may trigger a React eslint warning. Consider moving the fetch logic inside the `useEffect` callback or wrapping `fetchProduct` in `useCallback`.

## 6. Git Practices Coaching (Non-Scoring)

- **Limited incremental commits for M3**: The entire Milestone 3 implementation appears in just 2 commits (`31cdaee` and `1b360df` with identical messages: "M3: Add product list and detail pages with .NET API integration") plus one reorganization commit (`4f99151`). Professional practice favors smaller, incremental commits — e.g., separate commits for the Product model, controller, ProductList component, ProductDetail component, and integration work.

- **Duplicate commit messages**: Commits `31cdaee` and `1b360df` have identical messages, suggesting either a rebase issue or repeated work. Each commit should have a distinct, descriptive message reflecting its specific changes.

- **Generic early commit messages**: Earlier commits use messages like "Add files via upload" and "Create docs folder" that don't describe what was actually changed. Aim for messages that explain _what_ and _why_ (e.g., "Add ADR documents for technology decisions").

---

**25/25** — Excellent submission. All backend API endpoints function correctly with proper status codes; frontend fetches live data with robust loading, error, and empty states; bidirectional navigation between list and detail pages works well. The only gap is the lack of Atomic Design component hierarchy in the React code. The coaching notes above (hardcoded API URL, base64 images, CORS policy, git practices) are suggestions for professional growth, not scoring deductions.
