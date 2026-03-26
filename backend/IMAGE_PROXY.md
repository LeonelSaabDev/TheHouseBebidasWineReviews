# Image Proxy

Se agrego el endpoint `GET /api/images/proxy?url=...` para evitar fallas intermitentes de carga cuando Google Drive responde con bloqueos CORS, redirecciones o variaciones de host.

## Reglas de seguridad implementadas

- Solo se aceptan URLs absolutas con `http` o `https`.
- Hosts permitidos (allowlist estricta):
  - `drive.google.com`
  - `drive.usercontent.google.com`
  - `lh3.googleusercontent.com`
  - `images.unsplash.com`
- Se bloquea `localhost`, `.localhost`, `.local`, loopback y rangos privados IPv4/IPv6.
- Se valida DNS para evitar resolver hacia IPs privadas.

## Comportamiento

- Timeout de upstream: 10 segundos.
- Descarga en modo stream (`ResponseHeadersRead`) y reenvio con `content-type` de imagen.
- Si upstream no devuelve `image/*`, responde `502 Bad Gateway`.
- Cache-control: `public,max-age=300`.

## Integracion frontend

El frontend genera candidatos de imagen proxificados para links de Google Drive y conserva fallback a URL directa para mantener compatibilidad con registros existentes.
