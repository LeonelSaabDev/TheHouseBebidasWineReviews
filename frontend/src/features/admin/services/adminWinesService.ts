import { getWineDetail } from '../../../services/winesService'
import { apiRequest } from '../../../services/apiClient'
import type { AdminWineItem, AdminWinePayload } from '../types'

export function getAdminWineDetail(wineId: string) {
  return getWineDetail(wineId)
}

export function createAdminWine(payload: AdminWinePayload) {
  return apiRequest<AdminWineItem>('/api/admin/wines', {
    method: 'POST',
    body: payload,
    auth: true,
  })
}

export function updateAdminWine(wineId: string, payload: AdminWinePayload) {
  return apiRequest<AdminWineItem>(`/api/admin/wines/${wineId}`, {
    method: 'PUT',
    body: payload,
    auth: true,
  })
}

export function deleteAdminWine(wineId: string) {
  return apiRequest<void>(`/api/admin/wines/${wineId}`, {
    method: 'DELETE',
    auth: true,
  })
}
