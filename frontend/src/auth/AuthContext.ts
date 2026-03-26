import { createContext, useContext } from 'react'
import type { AdminLoginRequestDto, AdminLoginResponseDto } from './types'

export interface AuthContextValue {
  isAuthenticated: boolean
  session: AdminLoginResponseDto | null
  login: (credentials: AdminLoginRequestDto) => Promise<void>
  logout: () => void
}

export const AuthContext = createContext<AuthContextValue | null>(null)

export function useAuth() {
  const context = useContext(AuthContext)

  if (!context) {
    throw new Error('useAuth must be used within AuthProvider')
  }

  return context
}
