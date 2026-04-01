# CartController Integration Tests

## Summary

This test suite provides **xUnit integration tests** for the `CartController` using
`WebApplicationFactory<Program>`. Each test runs against a real ASP.NET Core pipeline
(routing, model binding, content negotiation) but swaps the production SQLite database
for an **EF Core in-memory database** so tests are fast, isolated, and require no
external infrastructure.

| Component | Technology |
|---|---|
| Test Framework | xUnit 2.9 |
| HTTP Testing | `Microsoft.AspNetCore.Mvc.Testing` (`WebApplicationFactory`) |
| Database | EF Core In-Memory provider (per-factory unique DB name) |
| Seeded Data | One `Product` (Id = 1) so cart operations have a valid product |

---

## Test Scenarios

| # | HTTP Method & Route | Scenario | Expected Status | Test Method |
|---|---|---|---|---|
| 1 | `GET /api/cart` | Retrieve (or auto-create) the cart for the hardcoded user | **200 OK** | `GetCart_Returns200` |
| 2 | `POST /api/cart` | Add a valid product (Id = 1) with quantity 2 | **201 Created** | `AddToCart_ValidProduct_Returns201` |
| 3a | `POST /api/cart` | Add a product whose Id does not exist (9999) | **404 Not Found** | `AddToCart_InvalidProductId_Returns404` |
| 3b | `POST /api/cart` | Add a product with quantity = 0 (invalid) | **400 Bad Request** | `AddToCart_ZeroQuantity_Returns400` |
| 4 | `PUT /api/cart/{id}` | Update an existing cart item's quantity to 5 | **200 OK** | `UpdateCartItem_Returns200WithUpdatedQuantity` |
| 5 | `DELETE /api/cart/{id}` | Remove a single item from the cart | **200 OK** | `RemoveCartItem_Returns200` |
| 6 | `DELETE /api/cart/clear` | Clear all items from the cart | **200 OK** | `ClearCart_Returns200` |

---

## How to Run

```bash
cd backend/BuckeyeMarketplaceAPI.Tests
dotnet test
```

Or from the solution root:

```bash
dotnet test BuckeyeMarketplace.sln
```

---

## File Structure

```
backend/
  BuckeyeMarketplaceAPI.Tests/
    BuckeyeMarketplaceAPI.Tests.csproj   # Test project with xUnit + MVC Testing packages
    CustomWebApplicationFactory.cs        # Swaps SQLite → In-Memory DB, seeds a Product
    CartControllerTests.cs                # 6 integration test methods
  BuckeyeMarketplaceAPI/
    Program.cs                            # Added `public partial class Program { }` for testability
```
