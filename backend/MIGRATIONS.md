# EF Core Migrations Guide

Este archivo deja comandos listos para generar/aplicar migraciones cuando se habilite ese paso.

## Requisitos

- Tener .NET SDK instalado.
- Ejecutar comandos desde la carpeta `backend/`.
- Mantener esta convención de config:
  - `TheHouseBebidas.WineReviews.Api/appsettings.json`: base compartida sin secretos.
  - `TheHouseBebidas.WineReviews.Api/appsettings.Development.json`: overrides no sensibles.
  - `dotnet user-secrets` o variables de entorno para valores sensibles (`Jwt:Key`, `AdminSeed:Password`).

## Configuración local (Development)

1) Inicializar user-secrets en el proyecto API (una sola vez por máquina):

```bash
dotnet user-secrets init --project TheHouseBebidas.WineReviews.Api
```

2) Cargar secretos locales:

```bash
dotnet user-secrets set "Jwt:Key" "TU_CLAVE_LARGA_DE_AL_MENOS_32_CARACTERES" --project TheHouseBebidas.WineReviews.Api
dotnet user-secrets set "AdminSeed:Password" "TU_PASSWORD_ADMIN_LOCAL" --project TheHouseBebidas.WineReviews.Api
```

3) Verificar secretos configurados:

```bash
dotnet user-secrets list --project TheHouseBebidas.WineReviews.Api
```

En `TheHouseBebidas.WineReviews.Api/appsettings.Development.json` debe existir:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=TU_SERVIDOR_SQL;Database=TheHouseBebidasWineReviews;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  },
  "AdminSeed": {
    "Username": "admin.dev",
    "Password": "__SET_VIA_USER_SECRETS_OR_ENV__",
    "IsActive": true
  }
}
```

Opcional por CI/devcontainer: usar variables de entorno con doble guion bajo (`__`):

```bash
setx Jwt__Key "TU_CLAVE_LARGA_DE_AL_MENOS_32_CARACTERES"
setx AdminSeed__Password "TU_PASSWORD_ADMIN_LOCAL"
```

## Definir entorno Development para EF

PowerShell:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
```

CMD:

```cmd
set ASPNETCORE_ENVIRONMENT=Development
```

## Instalar herramienta EF (si no esta instalada)

```bash
dotnet tool install --global dotnet-ef
```

Si ya la tenés instalada y querés actualizarla:

```bash
dotnet tool update --global dotnet-ef
```

## Crear migración inicial

```bash
dotnet ef migrations add InitialCreate --project TheHouseBebidas.WineReviews.Infrastructure --startup-project TheHouseBebidas.WineReviews.Api --output-dir Persistence/Migrations
```

## Aplicar migraciones a la base

```bash
dotnet ef database update --project TheHouseBebidas.WineReviews.Infrastructure --startup-project TheHouseBebidas.WineReviews.Api -- --environment Development
```

## Crear script SQL de migraciones (opcional)

```bash
dotnet ef migrations script --project TheHouseBebidas.WineReviews.Infrastructure --startup-project TheHouseBebidas.WineReviews.Api --idempotent --output migration.sql
```

## Comandos útiles

Listar migraciones:

```bash
dotnet ef migrations list --project TheHouseBebidas.WineReviews.Infrastructure --startup-project TheHouseBebidas.WineReviews.Api
```

Remover última migración (solo si aún no fue aplicada):

```bash
dotnet ef migrations remove --project TheHouseBebidas.WineReviews.Infrastructure --startup-project TheHouseBebidas.WineReviews.Api
```
