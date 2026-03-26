# E2E (Playwright)

Regresión E2E mínima para flujo crítico público + admin.

## Precondiciones

- Backend API corriendo y accesible (default esperado: `https://localhost:7095`).
- Secretos de backend cargados para login admin (`Jwt:Key` y `AdminSeed:Password`).
- Dependencias instaladas en `frontend/`.

## Variables útiles

- `E2E_BASE_URL`: URL del frontend para Playwright (default: `http://127.0.0.1:4173`).
- `E2E_FRONTEND_PORT`: puerto del servidor Vite que levanta Playwright (default: `4173`).
- `VITE_PROXY_TARGET`: target del proxy Vite para `/api` (default: `http://localhost:5089`).
- `E2E_ADMIN_USERNAME`: usuario admin para test login (default: `admin.dev`).
- `E2E_ADMIN_PASSWORD`: password admin para test login (obligatoria para ejecutar flujo admin).

## Ejecutar

```bash
npm run test:e2e
```

## Cobertura crítica incluida

- Público:
  - Home carga y secciones base visibles (`Hero`, `Vinos`, `About`, `Footer`).
  - Apertura de detalle de vino desde listado.
  - Envío de reseña inválida (campos vacíos) con validaciones visibles.
  - Envío de reseña válida con aserción robusta.
- Admin:
  - Login en `/admin/login`.
  - Acceso a dashboard protegido `/admin`.
  - Carga de listado admin de vinos.
  - Logout y redireccion a login.

## Limitación conocida (review válida)

El submit válido de reseña puede fallar de forma legítima por condiciones de entorno (rate limit 429, seed mutable o estado de datos variable). El test no asume éxito absoluto: valida **éxito** o **feedback de error esperado** para evitar falsos negativos cuando el backend está sano pero restringe la operación.
