# QA Report - Technical Audit & Final Hardening

Date: 2026-03-25  
Project: The House Bebidas Wine Reviews  
Environment: Development

## Resumen Ejecutivo

- Se ejecutó auditoría técnica completa sin build de frontend: backend tests (`dotnet test`) + frontend (`npm run lint`, `npx tsc --noEmit`).
- Se ejecutó smoke API automatizado para endpoints críticos PRD (públicos, validaciones, rate limiting, auth/admin security).
- Se detectó 1 fallo real en `POST /api/wines/{id}/reviews` (respondía `500` en payload válido) y fue corregido.
- Tras la corrección, los casos críticos pasan con los status esperados (incluyendo `429` en ráfaga y `401/200` en endpoints admin protegidos).

## Evidencias

### 1) Auditoría técnica

- Backend: `dotnet test` -> **7 passed, 0 failed, 0 skipped**.
- Frontend lint: `npm run lint` -> **OK** (sin errores).
- Frontend typecheck: `npx tsc --noEmit` -> **OK** (sin errores).

### 2) Smoke API automatizado (Development)

Fuente: `QA_SMOKE_RESULTS.json`

- `GET /api/wines` -> **200**
- `GET /api/wines/{id}` -> **200**
- `GET /api/wines/{id}/reviews` -> **200**
- `POST /api/wines/{id}/reviews` (válido) -> **201**
- `POST /api/wines/{id}/reviews` (inválido sin comment) -> **400**
- `POST /api/wines/{id}/reviews` (inválido rating fuera de rango) -> **400**
- Rate limiting (ráfaga POST reviews) -> **429 observado** (secuencia: `201, 201, 429, 429, 429, 429, 429`)
- `POST /api/admin/auth/login` (credenciales válidas) -> **200** + token
- `POST /api/admin/auth/login` (credenciales inválidas) -> **401**
- `GET /api/admin/reviews` sin token -> **401**
- `GET /api/admin/reviews` con token -> **200**

## Hardening / Config Review

- Secretos sensibles en appsettings versionables:
  - `Jwt:Key` en `appsettings.json` = placeholder (`__SET_VIA_USER_SECRETS_OR_ENV__`) -> **OK**
  - `AdminSeed:Password` en `appsettings.Development.json` = placeholder (`__SET_VIA_USER_SECRETS_OR_ENV__`) -> **OK**
- Middleware global de errores activo -> **OK** (`UseMiddleware<GlobalExceptionHandlingMiddleware>()` registrado en pipeline).
- Rate limiter activo y aplicado a POST de reviews -> **OK**
  - Politica `PublicReviewCreatePolicy` configurada en `Program.cs`.
  - Endpoint `POST /api/wines/{wineId}/reviews` con `[EnableRateLimiting("PublicReviewCreatePolicy")]`.
- Seguridad admin por rol -> **OK**
  - Controllers admin (`AdminReviewsController`, `AdminWinesController`, `AdminSiteContentController`) usan `[Authorize(Roles = "Admin")]`.

## Issues Encontrados / Corregidos

### Issue #1 (Corregido)

- **Síntoma:** `POST /api/wines/{id}/reviews` válido devolvía `500`.
- **Causa raíz:** `CreatedAtAction(nameof(GetByWineIdAsync), ...)` no resolvía ruta por supresión de sufijo `Async` en nombre de acción MVC (`No route matches the supplied values`).
- **Fix aplicado:** renombrar la acción GET a `GetByWineId` y actualizar `CreatedAtAction(nameof(GetByWineId), ...)`.
- **Archivo:** `backend/TheHouseBebidas.WineReviews.Api/Controllers/ReviewsController.cs`.
- **Revalidación:** smoke automatizado post-fix confirma `201` en POST válido y resto de casos en estado esperado.

## Riesgos Residuales

- No se ejecutó suite de pruebas de integración E2E completa (solo smoke crítico PRD).
- No se evaluó performance/carga sostenida más allá de ráfaga corta para validar `429`.
- El smoke depende de datos de Development ya semillados (cubre funcionalidad critica, no escenarios exhaustivos).

## Recomendacion de Release Readiness

**GO (condicional)**

Condiciones recomendadas:

1. Mantener secretos solo por user-secrets/env vars en ambientes no locales.
2. Correr regresión funcional ampliada (E2E/UI) antes de release final.
3. Mantener monitoreo de errores 5xx y metrica de rate limiting en primer despliegue.
