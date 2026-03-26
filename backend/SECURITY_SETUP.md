# Security Setup (Development)

Guía corta para dejar el backend local sin secretos en archivos versionables.

## 1) Secretos requeridos

- `Jwt:Key`: clave JWT (mínimo 32 caracteres).
- `AdminSeed:Password`: password del admin de desarrollo (obligatorio para crear admin nuevo o para rotacion).
- `AdminSeed:RotatePasswordIfExists`: controla si se rota password cuando el admin ya existe.

## 2) Inicializar user-secrets en API

```bash
dotnet user-secrets init --project TheHouseBebidas.WineReviews.Api
```

## 3) Cargar secretos locales

```bash
dotnet user-secrets set "Jwt:Key" "TU_CLAVE_LARGA_DE_AL_MENOS_32_CARACTERES" --project TheHouseBebidas.WineReviews.Api
dotnet user-secrets set "AdminSeed:Password" "TU_PASSWORD_ADMIN_LOCAL" --project TheHouseBebidas.WineReviews.Api
```

## 4) Verificar carga

```bash
dotnet user-secrets list --project TheHouseBebidas.WineReviews.Api
```

Debés ver ambas claves. Si falta `Jwt:Key`, la API no arranca y falla con error de configuración.

## 5) Prioridad de configuración

En Development, ASP.NET Core toma valores en este orden (ultimo gana):

1. `appsettings.json`
2. `appsettings.Development.json`
3. user-secrets
4. variables de entorno

Esto permite placeholders en repo y secretos reales solo en máquina local/entorno.

## 6) Comportamiento del seeder admin

- Si el admin (`AdminSeed:Username`) no existe, el seeder lo crea y requiere `AdminSeed:Password`.
- Si el admin ya existe, siempre sincroniza `AdminSeed:IsActive` (activa/desactiva usuario).
- Si `AdminSeed:RotatePasswordIfExists=true`, tambien rota password del usuario existente usando `AdminSeed:Password`.
- Si `RotatePasswordIfExists=true` pero falta `AdminSeed:Password`, no rota password y deja warning en logs.
- Defaults recomendados:
  - `appsettings.json`: `AdminSeed:RotatePasswordIfExists=false`
  - `appsettings.Development.json`: `AdminSeed:RotatePasswordIfExists=true`

## 7) Checklist rápido

- [ ] `appsettings.json` sin secretos reales.
- [ ] `appsettings.Development.json` sin secretos reales.
- [ ] `dotnet user-secrets list --project TheHouseBebidas.WineReviews.Api` muestra `Jwt:Key` y `AdminSeed:Password`.
- [ ] `dotnet ef database update --project TheHouseBebidas.WineReviews.Infrastructure --startup-project TheHouseBebidas.WineReviews.Api` ejecuta OK.
- [ ] `POST /api/admin/auth/login` responde `200` con credenciales válidas.
- [ ] `GET /api/admin/reviews` sin token responde `401`.
- [ ] `GET /api/admin/reviews` con token responde `200`.
