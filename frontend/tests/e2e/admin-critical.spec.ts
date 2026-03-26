import { expect, test } from '@playwright/test'

const adminUsername = process.env.E2E_ADMIN_USERNAME ?? 'admin.dev'
const adminPassword = process.env.E2E_ADMIN_PASSWORD ?? process.env.AdminSeed__Password

test.describe('Admin critical flow', () => {
  test('login + protected dashboard + list load + logout', async ({ page }) => {
    test.skip(!adminPassword, 'Set E2E_ADMIN_PASSWORD (or AdminSeed__Password) to run admin login E2E.')

    await page.goto('/admin/login')
    await expect(page).toHaveURL(/\/admin\/login$/)

    await page.getByLabel('Username').fill(adminUsername)
    await page.getByLabel('Password').fill(adminPassword ?? '')
    await page.getByRole('button', { name: 'Iniciar sesion' }).click()

    await expect(page).toHaveURL(/\/admin$/)
    await expect(page.getByRole('heading', { name: 'Panel operativo autenticado' })).toBeVisible()
    await expect(page.locator('.admin-status .state--success')).toHaveText('Autenticado')

    await expect(page.getByRole('heading', { name: 'Vinos' })).toBeVisible()
    await expect(page.getByText(/Mostrando \d+ de \d+ vinos\./)).toBeVisible()

    await page.getByRole('button', { name: 'Cerrar sesion' }).click()
    await expect(page).toHaveURL(/\/admin\/login$/)
    await expect(page.getByRole('heading', { name: 'Admin Login' })).toBeVisible()
  })
})
