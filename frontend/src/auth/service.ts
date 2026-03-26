import { apiRequest } from '../services/apiClient'
import type { AdminLoginRequestDto, AdminLoginResponseDto } from './types'

export function loginAdmin(payload: AdminLoginRequestDto) {
  return apiRequest<AdminLoginResponseDto>('/api/admin/auth/login', {
    method: 'POST',
    body: payload,
  })
}
