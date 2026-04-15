# E2E Test Run — Playwright Happy-Path Checkout

## Overview

This document replaces the former manual test plan. A single Playwright spec
(`frontend/e2e/checkout.spec.ts`) now exercises the full happy-path flow
end-to-end against the real backend and frontend.

### Flow under test

1. **Register** a new account (unique email per run)
2. **Browse** the product catalogue
3. **Add to cart** from the product-detail page
4. **Checkout** — enter a shipping address and place the order
5. **Verify** the order-confirmation page

---

## Prompts Given to Copilot

| # | Prompt (paraphrased) |
|---|----------------------|
| 1 | _"Replace what used to be the manual test plan with one Playwright MCP-driven E2E run of the happy path: register → login → browse → add to cart → checkout → view order in history. Commit the spec file and a short `docs/e2e-run.md`."_ |

---

## First-Run Failures & Corrections

| # | Failure | Root Cause | Fix |
|---|---------|-----------|-----|
| 1 | `expect(locator).toContainText()` — strict mode violation: `locator('h1')` resolved to **2 elements** (the header `<h1 class="header-title">` and the product-detail page `<h1>`). | The generic `h1` selector was ambiguous — Playwright found the always-present header title as well as the page-level heading. The navigation from product-detail to `/cart` was in progress and the previous page's DOM was still mounted. | Scoped the assertion to `.cart-container h1` instead of bare `h1`, replaced `page.click('.cart-button')` with `page.locator('.cart-button').click()`, and added an explicit `page.waitForURL(/\/cart/)` plus `expect(cartContainer).toBeVisible()` guard before checking the heading. |

**Run 2 → ✅ 1 passed (12.3 s)**

---

## How to Run

```bash
# From the repository root
cd frontend

# Install dependencies (first time only)
npm install
npx playwright install chromium

# Run the E2E test (starts both servers automatically via webServer config)
npx playwright test

# Or, if the servers are already running:
npx playwright test --reporter=list
```

### Prerequisites

| Dependency | Version |
|------------|---------|
| Node.js    | ≥ 18    |
| .NET SDK   | 10.0+   |
| Playwright | 1.59+   |

The `playwright.config.ts` defines two `webServer` entries that will
auto-start the backend (`dotnet run`) and frontend (`npm start`) if they
aren't already listening on their respective ports.

---

## Test Results

_Results are filled in after execution._

| Run | Date       | Pass/Fail | Duration | Notes                                       |
|-----|------------|-----------|----------|---------------------------------------------|
| 1   | Run 1      | ❌ Fail   | 2.6 s    | `h1` strict-mode violation (2 elements)     |
| 2   | Run 2      | ✅ Pass   | 12.3 s   | Scoped selector to `.cart-container h1`     |

---

## Selector Reference

The spec relies on **CSS class names** and **element IDs** already present in
the React components (no `data-testid` attributes were added):

| Element              | Selector                                      |
|----------------------|-----------------------------------------------|
| Full Name input      | `#register-fullname`                          |
| Email input          | `#register-email`                             |
| Password input       | `#register-password`                          |
| Confirm Password     | `#register-confirm`                           |
| Create Account btn   | `button:has-text("Create Account")`           |
| Header greeting      | `.header-greeting`                            |
| Products grid        | `.products-grid`                              |
| Product card link    | `.product-card-link`                          |
| Add to Cart button   | `.add-to-cart-btn-detail`                     |
| Cart button (header) | `.cart-button`                                |
| Cart badge           | `.cart-badge`                                 |
| Cart item row        | `.cart-item`                                  |
| Shipping address     | `#shipping-address`                           |
| Place Order button   | `.checkout-btn`                               |
| Success title        | `.order-success-title`                        |
| Order details        | `.order-details`                              |
| Shipping on confirm  | `.order-shipping-address`                     |
