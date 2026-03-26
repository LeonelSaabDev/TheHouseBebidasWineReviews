import type { ReviewDto, SiteContentDto, WineDetailDto } from '../../../types/api'

export interface AdminWinePayload {
  name: string
  winery: string
  year: number
  grapeVariety: string
  description: string
  imageUrl: string
  secondaryImageUrl: string | null
  featuredReviewSummary: string | null
  isActive: boolean
}

export interface AdminSiteContentPayload {
  title: string
  content: string
}

export interface AdminReviewsQuery {
  wineId?: string
  isVisible?: boolean
  rating?: number
  createdFrom?: string
  createdTo?: string
  [key: string]: string | number | boolean | undefined
}

export type AdminReviewItem = ReviewDto
export type AdminSiteContentItem = SiteContentDto
export type AdminWineItem = WineDetailDto
