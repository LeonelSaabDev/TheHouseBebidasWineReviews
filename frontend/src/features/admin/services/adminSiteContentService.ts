import { apiRequest } from '../../../services/apiClient'
import type { AdminSiteContentItem, AdminSiteContentPayload } from '../types'

export function updateAdminSiteContent(key: string, payload: AdminSiteContentPayload) {
  return apiRequest<AdminSiteContentItem>(`/api/admin/site-content/${key}`, {
    method: 'PUT',
    body: payload,
    auth: true,
  })
}
