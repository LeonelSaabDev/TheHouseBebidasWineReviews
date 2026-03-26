import { clearStoredAuthToken, getStoredAuthToken, isTokenExpired } from '../auth/storage'

type Primitive = string | number | boolean

interface ApiRequestOptions extends Omit<RequestInit, 'body'> {
  query?: Record<string, Primitive | undefined>
  body?: unknown
  auth?: boolean
}

const API_BASE_URL =
  import.meta.env.VITE_API_BASE_URL?.toString() ?? 'https://localhost:7095'

function redirectToAdminLogin() {
  if (typeof window === 'undefined') {
    return
  }

  if (window.location.pathname === '/admin/login') {
    return
  }

  window.location.replace('/admin/login')
}

function buildStatusMessage(status: number) {
  switch (status) {
    case 401:
      return 'Sesión expirada o credenciales inválidas. Volvé a iniciar sesión.'
    case 403:
      return 'No tenés permisos para realizar esta acción.'
    case 429:
      return 'Demasiadas solicitudes en poco tiempo. Espera unos segundos e intenta de nuevo.'
    default:
      if (status >= 500) {
        return 'Error interno del servidor. Intenta nuevamente en unos minutos.'
      }

      return `Request failed with status ${status}`
  }
}

function buildUrl(path: string, query?: Record<string, Primitive | undefined>) {
  const normalizedPath = path.startsWith('/') ? path : `/${path}`
  const url = new URL(`${API_BASE_URL}${normalizedPath}`)

  if (query) {
    Object.entries(query).forEach(([key, value]) => {
      if (value !== undefined && value !== '') {
        url.searchParams.set(key, String(value))
      }
    })
  }

  return url.toString()
}

async function parseError(response: Response) {
  const contentType = response.headers.get('content-type')
  const fallbackMessage = buildStatusMessage(response.status)

  if (contentType?.includes('application/json')) {
    try {
      const data = (await response.json()) as { detail?: string; title?: string }
      return data.detail ?? data.title ?? fallbackMessage
    } catch {
      return fallbackMessage
    }
  }

  return fallbackMessage
}

export async function apiRequest<T>(path: string, options: ApiRequestOptions = {}): Promise<T> {
  const { query, body, headers, auth = false, ...rest } = options
  let requestHeaders: HeadersInit = {
    ...headers,
  }

  if (body !== undefined) {
    requestHeaders = {
      'Content-Type': 'application/json',
      ...requestHeaders,
    }
  }

  if (auth) {
    const token = getStoredAuthToken()

    if (token) {
      const expired = isTokenExpired(token.expiresAtUtc)

      if (expired) {
        clearStoredAuthToken()
        redirectToAdminLogin()
        throw new Error('Sesión expirada. Volvé a iniciar sesión para continuar.')
      } else {
        requestHeaders = {
          ...requestHeaders,
          Authorization: `${token.tokenType || 'Bearer'} ${token.accessToken}`,
        }
      }
    }
  }

  const response = await fetch(buildUrl(path, query), {
    ...rest,
    headers: requestHeaders,
    body: body ? JSON.stringify(body) : undefined,
  })

  if (!response.ok) {
    if (auth && (response.status === 401 || response.status === 403)) {
      clearStoredAuthToken()
      redirectToAdminLogin()
    }

    throw new Error(await parseError(response))
  }

  if (response.status === 204) {
    return undefined as T
  }

  return (await response.json()) as T
}
