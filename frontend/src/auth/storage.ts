import type { AdminLoginResponseDto } from './types'

const ADMIN_AUTH_STORAGE_KEY = 'thb_admin_auth'

function canUseLocalStorage() {
  return typeof window !== 'undefined' && typeof window.localStorage !== 'undefined'
}

function isTokenShapeValid(token: AdminLoginResponseDto) {
  return Boolean(token.accessToken) && Boolean(token.expiresAtUtc) && Boolean(token.tokenType)
}

export function isTokenExpired(expiresAtUtc: string) {
  const expiresAt = Date.parse(expiresAtUtc)

  if (Number.isNaN(expiresAt)) {
    return true
  }

  return expiresAt <= Date.now()
}

export function getStoredAuthToken() {
  if (!canUseLocalStorage()) {
    return null
  }

  const rawToken = window.localStorage.getItem(ADMIN_AUTH_STORAGE_KEY)

  if (!rawToken) {
    return null
  }

  try {
    const parsedToken = JSON.parse(rawToken) as AdminLoginResponseDto

    if (!isTokenShapeValid(parsedToken)) {
      clearStoredAuthToken()
      return null
    }

    return parsedToken
  } catch {
    clearStoredAuthToken()
    return null
  }
}

export function setStoredAuthToken(token: AdminLoginResponseDto) {
  if (!canUseLocalStorage()) {
    return
  }

  window.localStorage.setItem(ADMIN_AUTH_STORAGE_KEY, JSON.stringify(token))
}

export function clearStoredAuthToken() {
  if (!canUseLocalStorage()) {
    return
  }

  window.localStorage.removeItem(ADMIN_AUTH_STORAGE_KEY)
}

export function isAuthenticated() {
  const token = getStoredAuthToken()

  if (!token) {
    return false
  }

  const authenticated = !isTokenExpired(token.expiresAtUtc)

  if (!authenticated) {
    clearStoredAuthToken()
  }

  return authenticated
}
