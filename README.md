# The House Bebidas Wine Reviews

Aplicacion fullstack para publicar y gestionar resenas de vinos de The House Bebidas.

Incluye:

- Sitio publico con catalogo, detalle de vinos y reseñas visibles.
- Publicacion de reseñas por visitantes (sin registro) con validaciones y rate limiting.
- Panel privado de administracion con autenticacion JWT para gestionar vinos, reseñas y contenido institucional.

## Stack tecnologico

### Backend

- .NET 9 (`ASP.NET Core Web API`)
- Arquitectura en N capas:
  - `TheHouseBebidas.WineReviews.Api` (presentacion/API)
  - `TheHouseBebidas.WineReviews.Application` (casos de uso, DTOs, interfaces)
  - `TheHouseBebidas.WineReviews.Domain` (entidades y reglas de dominio)
  - `TheHouseBebidas.WineReviews.Infrastructure` (EF Core, persistencia, JWT, seeding)
- Entity Framework Core 9 + SQL Server
- JWT Bearer para autenticacion de administrador
- Rate Limiting en endpoint publico de creacion de reseñas

### Frontend

- React 19 + TypeScript
- Vite
- React Router
- Playwright para E2E
- ESLint + TypeScript compiler para calidad estatica

### Base de datos y auth

- Base de datos: SQL Server
- ORM: Entity Framework Core (migraciones en `backend/TheHouseBebidas.WineReviews.Infrastructure/Migrations`)
- Auth: JWT para area admin (`role: Admin`)

## Arquitectura

## Backend (N capas)

```text
backend/
  TheHouseBebidas.WineReviews.Api/            -> Controllers, middleware, configuracion HTTP
  TheHouseBebidas.WineReviews.Application/    -> Servicios de aplicacion, DTOs, contratos
  TheHouseBebidas.WineReviews.Domain/         -> Entidades y reglas de negocio
  TheHouseBebidas.WineReviews.Infrastructure/ -> EF Core, repositorios, auth, seeding
  TheHouseBebidas.WineReviews.Tests/          -> Tests unitarios
```

## Frontend (feature-first + capas de UI)

```text
frontend/src/
  app/          -> App shell
  routes/       -> Ruteo y rutas protegidas
  pages/        -> Paginas de alto nivel
  features/     -> Modulos funcionales (home, wines, reviews, admin, auth)
  components/   -> Componentes reutilizables
  services/     -> Cliente API y servicios HTTP
  auth/         -> Contexto/estado de autenticacion admin
  hooks/ types/ utils/ styles/
```

## Requisitos previos

- .NET SDK 9
- Node.js 20+ y npm
- SQL Server accesible localmente
- Certificado de desarrollo HTTPS para ASP.NET Core (si se usa perfil `https`)

## Configuracion local

## 1) Backend: secretos obligatorios (user-secrets)

Desde la carpeta `backend/`:

```bash
dotnet user-secrets init --project TheHouseBebidas.WineReviews.Api
dotnet user-secrets set "Jwt:Key" "TU_CLAVE_DE_AL_MENOS_32_CARACTERES" --project TheHouseBebidas.WineReviews.Api
dotnet user-secrets set "AdminSeed:Password" "TU_PASSWORD_ADMIN_LOCAL" --project TheHouseBebidas.WineReviews.Api
dotnet user-secrets list --project TheHouseBebidas.WineReviews.Api
```

Notas:

- `Jwt:Key` es obligatoria y debe tener minimo 32 caracteres.
- `AdminSeed:Password` es obligatoria para crear/rotar password del admin seed.
- `appsettings.json` y `appsettings.Development.json` usan placeholders para no versionar secretos.

## 2) Backend: connection string

Configura `ConnectionStrings:DefaultConnection` en `backend/TheHouseBebidas.WineReviews.Api/appsettings.Development.json` segun tu SQL Server local.

## 3) Frontend: variables opcionales

El frontend funciona sin `.env` por defecto. Variables disponibles:

- `VITE_API_BASE_URL` (default: `https://localhost:7095`)
- `VITE_PROXY_TARGET` (default en Vite dev proxy: `http://localhost:5089`)

## Como correr el proyecto

## Backend

Desde `backend/`:

```bash
dotnet restore
dotnet run --project TheHouseBebidas.WineReviews.Api --launch-profile https
```

URLs por `launchSettings.json`:

- `https://localhost:7095`
- `http://localhost:5089`

## Frontend

Desde `frontend/`:

```bash
npm install
npm run dev
```

URL dev por defecto:

- `http://localhost:5173`

## Variables/config clave

### Backend

- `ConnectionStrings:DefaultConnection`: conexion SQL Server
- `Jwt:Issuer`: emisor del token
- `Jwt:Audience`: audiencia del token
- `Jwt:Key`: secreto JWT (obligatorio, >= 32 chars)
- `Jwt:ExpirationMinutes`: expiracion del access token
- `AdminSeed:Username`: usuario seed admin
- `AdminSeed:Password`: password seed admin (secreto)
- `AdminSeed:IsActive`: estado activo del admin
- `AdminSeed:RotatePasswordIfExists`: rota password si el usuario ya existe
- `ASPNETCORE_ENVIRONMENT`: entorno (`Development` en local)

### Frontend

- `VITE_API_BASE_URL`: base URL para requests del cliente HTTP
- `VITE_PROXY_TARGET`: target del proxy Vite para ruta `/api` en dev server
- (E2E) `E2E_BASE_URL`, `E2E_FRONTEND_PORT`, `E2E_ADMIN_USERNAME`, `E2E_ADMIN_PASSWORD`

## Flujo de migraciones

Resumen rapido (desde `backend/`):

1. Cargar secretos (`Jwt:Key`, `AdminSeed:Password`).
2. Definir entorno `Development`.
3. Crear migracion con `dotnet ef migrations add ...`.
4. Aplicar con `dotnet ef database update ...`.

Referencia completa:

- `backend/MIGRATIONS.md`

## QA rapido

## Backend

Desde `backend/`:

```bash
dotnet test
```

## Frontend

Desde `frontend/`:

```bash
npm run lint
npx tsc --noEmit
```

## E2E basico (opcional, con backend arriba)

Desde `frontend/`:

```bash
npm run test:e2e
```

## Smoke minimo API (manual)

- `GET /api/wines` debe responder `200`.
- `POST /api/admin/auth/login` con credenciales validas debe responder `200`.
- `GET /api/admin/reviews` sin token debe responder `401`.

## Endpoints principales

## Publicos

- `GET /api/wines`
- `GET /api/wines/{id}`
- `GET /api/wines/{wineId}/reviews`
- `POST /api/wines/{wineId}/reviews` (con rate limiting)
- `GET /api/site-content`
- `GET /api/images/proxy?url=...`

### Query params destacados

- `GET /api/wines`: `searchTerm`, `minimumRating`, `maximumRating`, `sortBy`, `sortDescending`, `page`, `pageSize`
- `GET /api/admin/reviews`: `wineId`, `isVisible`, `rating`, `createdFrom`, `createdTo`

## Admin (requiere JWT con rol Admin, excepto login)

- `POST /api/admin/auth/login`
- `POST /api/admin/wines`
- `PUT /api/admin/wines/{id}`
- `DELETE /api/admin/wines/{id}`
- `GET /api/admin/reviews`
- `DELETE /api/admin/reviews/{id}`
- `PUT /api/admin/site-content/{key}`

## Notas de seguridad

- No guardes secretos en archivos versionados (`appsettings*.json` usan placeholders).
- `Jwt:Key` invalida o corta detiene el arranque del backend por seguridad.
- Endpoints admin protegidos con `[Authorize(Roles = "Admin")]`.
- CORS restringido a orígenes locales definidos en `Program.cs`.
- Rate limiting activo para `POST /api/wines/{wineId}/reviews`.
- `GET /api/images/proxy` aplica allowlist de hosts y controles anti-SSRF.

## Estado actual del proyecto

- Backend, frontend y base de datos integrados en entorno de desarrollo.
- Auditoria tecnica final documentada en `FINAL_TECH_AUDIT.md`.
- QA tecnico reportado en `QA_REPORT.md` (lint/tsc/tests/smoke de endpoints criticos).
- Estructura lista para evolucionar funcionalidades y hardening adicional previo a release final.
