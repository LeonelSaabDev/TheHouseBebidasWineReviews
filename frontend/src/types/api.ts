export interface WineListItemDto {
  id: string
  name: string
  winery: string
  year: number
  grapeVariety: string
  imageUrl: string
  secondaryImageUrl: string | null
  averageRating: number
  reviewsCount: number
  featuredReviewSummary: string | null
}

export interface WineDetailDto {
  id: string
  name: string
  winery: string
  year: number
  grapeVariety: string
  description: string
  imageUrl: string
  secondaryImageUrl: string | null
  averageRating: number
  reviewsCount: number
  featuredReviewSummary: string | null
  isActive: boolean
}

export interface ReviewDto {
  id: string
  wineId: string
  authorName: string
  comment: string
  rating: number
  createdAt: string
  isVisible: boolean
}

export interface CreateReviewRequestDto {
  authorName: string
  comment: string
  rating: number
}

export interface SiteContentDto {
  id: string
  key: string
  title: string
  content: string
  updatedAt: string
}

export interface WineListQuery {
  searchTerm?: string
  sortBy?: 'rating' | 'name' | 'year'
  sortDescending?: boolean
  page?: number
  pageSize?: number
  [key: string]: string | number | boolean | undefined
}

export interface PaginatedResponseDto<TItem> {
  items: TItem[]
  totalItems: number
  totalPages: number
  page: number
  pageSize: number
}
