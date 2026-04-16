# Changelog — BuckeyeMarketplace

## [2026-04-15] Security & Data-Integrity Fixes

### 1. Parameterize all product queries via EF Core LINQ (`cf3f088`)

**Bug:** `ProductsController` used a static in-memory `List<Product>` for all
CRUD operations while `CartController` and `OrderController` validated against
the real database. This caused data inconsistency — products created or deleted
at runtime were invisible to cart/order workflows, and vice versa.

**Fix:** Refactored `ProductsController` to inject `AppDbContext` and use EF Core
LINQ methods (`.FindAsync()`, `.ToListAsync()`, `.Add()`, `.Remove()` +
`.SaveChangesAsync()`). EF Core automatically generates parameterized SQL for
every query, eliminating any risk of SQL injection. Removed the dead
`GetProductById()` static helper.

**Files changed:** `Controllers/ProductsController.cs`

---

### 2. Move secrets to `dotnet user-secrets`, add fail-fast guards (`7d57d5d`)

**Bug:** `ConnectionStrings:DefaultConnection` was hardcoded in
`appsettings.json` and committed to the Git repository. The JWT signing key
error messages gave developers no guidance on how to configure the missing
secret.

**Fix:**
- Moved the connection string to `dotnet user-secrets` (UserSecretsId
  `7594013c-c9ff-44f5-878a-13cf690c6142`).
- Removed the `ConnectionStrings` block from `appsettings.json`.
- Added a fail-fast guard in `Program.cs` that throws immediately on startup if
  the connection string is missing, with a message telling developers to run
  `dotnet user-secrets set`.
- Updated JWT Key error messages in both `Program.cs` and `AuthController.cs`
  (`GenerateJwtToken` + `GetPrincipalFromExpiredToken`) to reference the
  `dotnet user-secrets set` command.

**Files changed:** `appsettings.json`, `Program.cs`, `Controllers/AuthController.cs`

---

### 3. Add secure response headers + fix IDOR in cart endpoints (`38dbdba`)

#### 3a. Secure Headers

**Bug:** API responses contained no security headers, leaving the application
vulnerable to clickjacking, MIME-type sniffing, and referrer leakage.

**Fix:** Added inline middleware in `Program.cs` that sets the following headers
on every HTTP response:

| Header | Value |
|--------|-------|
| `X-Content-Type-Options` | `nosniff` |
| `X-Frame-Options` | `DENY` |
| `X-XSS-Protection` | `0` |
| `Referrer-Policy` | `strict-origin-when-cross-origin` |
| `Permissions-Policy` | `accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), usb=()` |
| `Content-Security-Policy` | `default-src 'none'; frame-ancestors 'none'` |
| `Cache-Control` / `Pragma` | `no-store` / `no-cache` |

#### 3b. Insecure Direct Object Reference (IDOR) in CartController

**Bug:** `PUT /api/cart/{cartItemId}` and `DELETE /api/cart/{cartItemId}` used
`_context.CartItems.FindAsync(cartItemId)` without verifying that the cart item
belonged to the authenticated user. Any logged-in user could modify or delete
another user's cart items by guessing the numeric ID.

**Fix:** Replaced the bare `FindAsync` calls with `FirstOrDefaultAsync` queries
that join through the `Cart` navigation property:

```csharp
var item = await _context.CartItems
    .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.Cart.UserId == userId);
```

The `userId` is extracted from the JWT `ClaimTypes.NameIdentifier` claim. If the
item does not exist **or** belongs to a different user, the endpoint returns
`404 Not Found` with no information leakage.

**Files changed:** `Controllers/CartController.cs`

---

### Verification

All fixes verified with:
- `dotnet build` — **0 warnings, 0 errors**
- `dotnet test` — **99/99 tests passing**
- Live `curl` — all 7 security headers confirmed on HTTP responses
