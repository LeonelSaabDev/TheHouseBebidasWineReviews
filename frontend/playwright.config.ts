import { defineConfig, devices } from '@playwright/test'

const PORT = Number(process.env.E2E_FRONTEND_PORT ?? 4173)
const BASE_URL = process.env.E2E_BASE_URL ?? `http://127.0.0.1:${PORT}`

export default defineConfig({
  testDir: './tests/e2e',
  timeout: 45_000,
  expect: {
    timeout: 8_000,
  },
  fullyParallel: true,
  retries: process.env.CI ? 1 : 0,
  reporter: [['list']],
  use: {
    baseURL: BASE_URL,
    trace: 'retain-on-failure',
    screenshot: 'only-on-failure',
    video: 'retain-on-failure',
    ignoreHTTPSErrors: true,
  },
  webServer: {
    command: `npm run dev -- --host 127.0.0.1 --port ${PORT}`,
    port: PORT,
    timeout: 120_000,
    reuseExistingServer: true,
    env: {
      ...process.env,
      VITE_API_BASE_URL: BASE_URL,
    },
  },
  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },
  ],
})
