/**
 * checkout.spec.ts
 *
 * Playwright E2E happy-path test for Buckeye Marketplace:
 *   register → browse products → add to cart → checkout → order confirmation
 *
 * A unique email is generated per run so the test is idempotent against
 * a persistent (non-wiped) SQLite database.
 */
import { test, expect } from '@playwright/test';

/* ── Test data ─────────────────────────────────────────────── */
const uniqueEmail = `e2e_${Date.now()}@buckeyetest.com`;
const password     = 'Test1234!';   // meets Identity rules: 8+ chars, upper, lower, digit, symbol
const fullName     = 'Playwright Tester';
const shippingAddr = '123 High St, Columbus, OH 43210';

test.describe('Happy-path checkout flow', () => {

  test('register → browse → add to cart → checkout → order confirmation', async ({ page }) => {

    /* ────────────────────────────────────────────────────────
     * STEP 1 – Register a new account
     * ──────────────────────────────────────────────────────── */
    await page.goto('/register');
    await page.fill('#register-fullname', fullName);
    await page.fill('#register-email', uniqueEmail);
    await page.fill('#register-password', password);
    await page.fill('#register-confirm', password);
    await page.click('button:has-text("Create Account")');

    // After successful registration the app redirects to "/"
    await expect(page).toHaveURL('http://localhost:3000/', { timeout: 20_000 });

    // Header should now greet the user
    await expect(page.locator('.header-greeting')).toContainText('Hi,', { timeout: 10_000 });

    /* ────────────────────────────────────────────────────────
     * STEP 2 – Browse products on the home page
     * ──────────────────────────────────────────────────────── */
    await expect(page.locator('.products-grid')).toBeVisible({ timeout: 10_000 });

    // Click the first product card to navigate to its detail page
    const firstCard = page.locator('.product-card-link').first();
    await expect(firstCard).toBeVisible();
    await firstCard.click();

    /* ────────────────────────────────────────────────────────
     * STEP 3 – Add the product to the cart
     * ──────────────────────────────────────────────────────── */
    const addBtn = page.locator('.add-to-cart-btn-detail');
    await expect(addBtn).toBeVisible({ timeout: 10_000 });
    await expect(addBtn).toBeEnabled();          // not "Out of Stock"
    await addBtn.click();

    // Cart badge in header should appear / increment
    await expect(page.locator('.cart-badge')).toBeVisible({ timeout: 5_000 });

    /* ────────────────────────────────────────────────────────
     * STEP 4 – Navigate to the cart page
     * ──────────────────────────────────────────────────────── */
    await page.locator('.cart-button').click();
    await page.waitForURL(/\/cart/, { timeout: 15_000 });

    // Wait for the cart container to render (specific to the Cart page)
    const cartContainer = page.locator('.cart-container');
    await expect(cartContainer).toBeVisible({ timeout: 15_000 });
    await expect(cartContainer.locator('h1')).toContainText('Shopping Cart');

    // At least one cart item should be rendered
    await expect(page.locator('.cart-item').first()).toBeVisible({ timeout: 10_000 });

    /* ────────────────────────────────────────────────────────
     * STEP 5 – Fill shipping address and place order
     * ──────────────────────────────────────────────────────── */
    await page.fill('#shipping-address', shippingAddr);
    await page.click('.checkout-btn');

    /* ────────────────────────────────────────────────────────
     * STEP 6 – Verify order confirmation page
     * ──────────────────────────────────────────────────────── */
    await expect(page.locator('.order-success-title')).toContainText(
      'Order Placed Successfully!',
      { timeout: 20_000 },
    );

    // Order details section should be visible
    await expect(page.locator('.order-details')).toBeVisible();

    // Shipping address should appear on the confirmation
    await expect(page.locator('.order-shipping-address')).toContainText(shippingAddr);
  });
});
