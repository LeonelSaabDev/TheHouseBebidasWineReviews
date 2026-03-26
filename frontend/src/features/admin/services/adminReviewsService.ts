import { apiRequest } from '../../../services/apiClient'
import type { AdminReviewItem, AdminReviewsQuery } from '../types'

export function getAdminReviews(query: AdminReviewsQuery = {}) {
  return apiRequest<AdminReviewItem[]>('/api/admin/reviews', {
    method: 'GET',
    query,
    auth: true,
  })
}

export function deleteAdminReview(reviewId: string) {
  return apiRequest<void>(`/api/admin/reviews/${reviewId}`, {
    method: 'DELETE',
    auth: true,
  })
}
