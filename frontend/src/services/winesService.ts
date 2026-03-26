import type { PaginatedResponseDto, WineDetailDto, WineListItemDto, WineListQuery } from '../types/api'
import { apiRequest } from './apiClient'

export function getWines(query: WineListQuery = {}, signal?: AbortSignal) {
  return apiRequest<PaginatedResponseDto<WineListItemDto>>('/api/wines', {
    method: 'GET',
    query,
    signal,
  })
}

export function getWineDetail(wineId: string, signal?: AbortSignal) {
  return apiRequest<WineDetailDto>(`/api/wines/${wineId}`, {
    method: 'GET',
    signal,
  })
}
