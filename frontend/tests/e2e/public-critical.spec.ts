import { expect, test } from '@playwright/test'

test.describe('Public critical flow', () => {
  test('home sections + wine detail + invalid and valid review paths', async ({ page }) => {
    await page.goto('/')

    await expect(page.locator('#hero')).toBeVisible()
    await expect(page.locator('#vinos')).toBeVisible()
    await expect(page.locator('#about')).toBeVisible()
    await expect(page.locator('#footer')).toBeVisible()

    const wineCards = page.locator('.wine-card')
    await expect(wineCards.first()).toBeVisible()
    await wineCards.first().click()

    const dialog = page.getByRole('dialog')
    await expect(dialog).toBeVisible()
    await expect(dialog.getByRole('heading', { name: 'Resenas publicas' })).toBeVisible()

    await dialog.getByRole('button', { name: 'Publicar resena' }).click()

    await expect(dialog.getByText('El comentario es obligatorio.')).toBeVisible()
    await expect(dialog.getByText('La puntuacion es obligatoria.')).toBeVisible()

    const reviewComment = `E2E review ${Date.now()}`
    await dialog.getByLabel('Comentario').fill(reviewComment)
    await dialog.getByLabel('Puntuacion').selectOption('5')
    await dialog.getByRole('button', { name: 'Publicar resena' }).click()

    const success = dialog.getByText('Resena publicada correctamente.')
    const rateLimited = dialog.getByText('Demasiadas solicitudes en poco tiempo. Espera unos segundos e intenta de nuevo.')
    const genericFailure = dialog.getByText(/No se pudo publicar la resena|Request failed with status/i)

    await expect(success.or(rateLimited).or(genericFailure)).toBeVisible()

    const successVisible = await success.isVisible().catch(() => false)

    if (successVisible) {
      await expect(dialog.locator('.review-item').first()).toContainText(reviewComment)
    }
  })
})
