import { useState, type ReactNode } from 'react'
import { AuthContext, type AuthContextValue } from './AuthContext'
import { loginAdmin } from './service'
import {
  clearStoredAuthToken,
  getStoredAuthToken,
  isTokenExpired,
  setStoredAuthToken,
} from './storage'
import type { AdminLoginRequestDto, AdminLoginResponseDto } from './types'

function getInitialSession() {
  const storedToken = getStoredAuthToken()

  if (!storedToken) {
    return null
  }

  if (isTokenExpired(storedToken.expiresAtUtc)) {
    clearStoredAuthToken()
    return null
  }

  return storedToken
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [session, setSession] = useState<AdminLoginResponseDto | null>(() => getInitialSession())

  const login = async (credentials: AdminLoginRequestDto) => {
    const response = await loginAdmin(credentials)
    setStoredAuthToken(response)
    setSession(response)
  }

  const logout = () => {
    clearStoredAuthToken()
    setSession(null)
  }

  const value: AuthContextValue = {
    isAuthenticated: session ? !isTokenExpired(session.expiresAtUtc) : false,
    session,
    login,
    logout,
  }

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}
