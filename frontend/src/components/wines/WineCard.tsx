import { memo, useCallback, useMemo, useState } from 'react'
import type { WineListItemDto } from '../../types/api'
import { StarRating } from './StarRating'
import { getWineImageCandidates } from '../../utils/normalizeImageUrl'

interface WineCardProps {
  wine: WineListItemDto
  onSelect: (wineId: string) => void
  prioritizeImage?: boolean
}

function WineCardComponent({ wine, onSelect, prioritizeImage = false }: WineCardProps) {
  const openDetail = useCallback(() => onSelect(wine.id), [onSelect, wine.id])
  const imageCandidates = useMemo(() => getWineImageCandidates(wine.imageUrl), [wine.imageUrl])
  const [imageCandidateState, setImageCandidateState] = useState<{ source: string; index: number }>({
    source: wine.imageUrl,
    index: 0,
  })
  const imageCandidateIndex = imageCandidateState.source === wine.imageUrl ? imageCandidateState.index : 0
  const imageUrl = imageCandidates[imageCandidateIndex] ?? wine.imageUrl

  const handleKeyDown = useCallback((event: React.KeyboardEvent<HTMLElement>) => {
    if (event.key === 'Enter' || event.key === ' ') {
      event.preventDefault()
      openDetail()
    }
  }, [openDetail])

  return (
    <article className="wine-card" role="button" tabIndex={0} onClick={openDetail} onKeyDown={handleKeyDown}>
      <div className="wine-card__media">
        <img
        src={imageUrl}
        alt={`${wine.name} de ${wine.winery}`}
        className="wine-card__image"
        loading={prioritizeImage ? 'eager' : 'lazy'}
        fetchPriority={prioritizeImage ? 'high' : 'auto'}
        decoding="async"
        onError={() => {
          setImageCandidateState({
            source: wine.imageUrl,
            index: Math.min(imageCandidateIndex + 1, imageCandidates.length - 1),
          })
        }}
      />
      </div>
      <div className="wine-card__body">
        <p className="wine-card__meta">{wine.winery}</p>
        <h3>{wine.name}</h3>
        <p className="wine-card__variety">{wine.grapeVariety}</p>
        <p className="wine-card__summary">
          {wine.featuredReviewSummary ?? 'Descubrí este vino y lee la experiencia de otros amantes del vino.'}
        </p>
        <StarRating rating={wine.averageRating} reviewsCount={wine.reviewsCount} size="sm" />
      </div>
    </article>
  )
}

export const WineCard = memo(WineCardComponent)
