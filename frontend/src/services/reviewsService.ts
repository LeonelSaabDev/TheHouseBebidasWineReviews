import type { CreateReviewRequestDto, ReviewDto } from '../types/api'
import { apiRequest } from './apiClient'

export function getWineReviews(wineId: string, signal?: AbortSignal) {
  return apiRequest<ReviewDto[]>(`/api/wines/${wineId}/reviews`, {
    method: 'GET',
    signal,
  })
}

export function createWineReview(wineId: string, payload: CreateReviewRequestDto) {
  return apiRequest<ReviewDto>(`/api/wines/${wineId}/reviews`, {
    method: 'POST',
    body: payload,
  })
}
