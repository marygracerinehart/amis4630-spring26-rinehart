import { defineConfig } from '@playwright/test';

/**
 * Playwright configuration for BuckeyeMarketplace E2E tests.
 *
 * Expects both servers to be running:
 *   Backend  → http://localhost:5107  (dotnet run)
 *   Frontend → http://localhost:3000  (npm start)
 *
 * The webServer array will start them automatically if they aren't already up.
 */
export default defineConfig({
  testDir: './e2e',
  timeout: 90_000,               // 90 s per test (registration + checkout)
  expect: { timeout: 15_000 },
  fullyParallel: false,          // run serially – one shared DB
  retries: 0,
  reporter: [['list']],

  use: {
    baseURL: 'http://localhost:3000',
    headless: true,
    screenshot: 'only-on-failure',
    trace: 'on-first-retry',
    actionTimeout: 10_000,
  },

  webServer: [
    {
      command:
        'cd ../backend/BuckeyeMarketplaceAPI && dotnet run --launch-profile http',
      url: 'http://localhost:5107/api/products',
      reuseExistingServer: true,
      timeout: 120_000,
    },
    {
      command: 'BROWSER=none npm start',
      url: 'http://localhost:3000',
      reuseExistingServer: true,
      timeout: 120_000,
    },
  ],

  projects: [
    {
      name: 'chromium',
      use: { browserName: 'chromium' },
    },
  ],
});
