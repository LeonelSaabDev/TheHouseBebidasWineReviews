# Fase 2 - Manual Validation Checklist

Checklist manual para validar autenticación admin JWT, endpoints públicos y endpoints admin protegidos.

## 0) Precondiciones

- API levantada en local (ejemplo: `https://localhost:7196` o `http://localhost:5256`).
- Base de datos accesible por `ConnectionStrings:DefaultConnection` en `appsettings.Development.json`.
- `Jwt:Key` configurado por user-secrets o variable de entorno (`Jwt__Key`) con mínimo 32 caracteres.
- `AdminSeed:Password` configurado por user-secrets o variable de entorno (`AdminSeed__Password`).
- Usuario admin seed configurado (username/password válidos).
- Tener a mano un `wineId` existente para pruebas de reviews (GUID válido).

## 1) Login admin JWT

- [ ] `POST /api/admin/auth/login` con credenciales válidas.
  - Request ejemplo:
    ```json
    {
      "username": "admin",
      "password": "TuPasswordAdmin"
    }
    ```
  - Esperado: `200 OK` con `accessToken`, `tokenType` y `expiresAtUtc`.
- [ ] Guardar el `accessToken` para pruebas de endpoints protegidos.
- [ ] `POST /api/admin/auth/login` con password incorrecto.
  - Esperado: `401 Unauthorized`.

## 2) Endpoints públicos (wines/reviews/site-content)

- [ ] `GET /api/wines`.
  - Esperado: `200 OK` y lista (puede estar vacía).
- [ ] `GET /api/wines?sortBy=rating&sortDescending=true`.
  - Esperado: `200 OK`.
- [ ] `GET /api/wines/{wineId}` con GUID existente.
  - Esperado: `200 OK`.
- [ ] `GET /api/wines/{wineId}/reviews` con GUID existente.
  - Esperado: `200 OK`.
- [ ] `GET /api/site-content`.
  - Esperado: `200 OK`.
- [ ] `POST /api/wines/{wineId}/reviews` con payload válido.
  - Request ejemplo:
    ```json
    {
      "comment": "Excelente vino para una cena.",
      "rating": 5
    }
    ```
  - Esperado: `201 Created`.

## 3) Endpoints admin protegidos

- [ ] `GET /api/admin/reviews` sin token.
  - Esperado: `401 Unauthorized`.
- [ ] `GET /api/admin/reviews` con `Authorization: Bearer {accessToken}`.
  - Esperado: `200 OK`.
- [ ] `POST /api/admin/wines` con token admin y payload válido.
  - Esperado: `201 Created`.
- [ ] `PUT /api/admin/wines/{id}` con token admin y payload válido.
  - Esperado: `200 OK` (o `404 Not Found` si el id no existe).
- [ ] `DELETE /api/admin/wines/{id}` con token admin.
  - Esperado: `204 No Content` (o `404 Not Found` si el id no existe).
- [ ] `PUT /api/admin/site-content/{key}` con token admin y payload válido.
  - Esperado: `200 OK` (o `404 Not Found` si la key no existe).

## 4) Casos de error clave

- [ ] Validación de review - rating fuera de rango.
  - `POST /api/wines/{wineId}/reviews` con `rating: 6`.
  - Esperado: `400 Bad Request` con `application/problem+json` y detalle de rating fuera de `1..5`.
- [ ] Validación de review - comment vacío.
  - `POST /api/wines/{wineId}/reviews` con `comment: "   "`.
  - Esperado: `400 Bad Request` con `application/problem+json` y mensaje de comentario requerido.
- [ ] Unauthorized admin endpoint.
  - `POST /api/admin/wines` sin `Authorization`.
  - Esperado: `401 Unauthorized`.
- [ ] Unauthorized por token inválido/malformado.
  - `GET /api/admin/reviews` con token inválido.
  - Esperado: `401 Unauthorized`.

## 5) Evidencia sugerida

- [ ] Guardar capturas o logs con status code + response body de cada caso.
- [ ] Registrar fecha/hora y ambiente (`Development`).
- [ ] Si falla un caso, registrar endpoint, payload y respuesta completa.
