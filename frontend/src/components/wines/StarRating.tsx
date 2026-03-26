import { memo, useMemo } from 'react'

interface StarRatingProps {
  rating: number
  reviewsCount?: number
  size?: 'sm' | 'md'
}

function StarRatingComponent({ rating, reviewsCount, size = 'md' }: StarRatingProps) {
  const roundedRating = useMemo(() => rating.toFixed(1), [rating])
  const stars = useMemo(
    () =>
      Array.from({ length: 5 }, (_, index) => {
        const starValue = index + 1
        const filled = rating >= starValue - 0.25
        const half = !filled && rating >= starValue - 0.75

        return (
          <span key={starValue} className={`star ${filled ? 'star--filled' : ''} ${half ? 'star--half' : ''}`}>
            <span>★</span>
          </span>
        )
      }),
    [rating],
  )

  return (
    <div className={`star-rating star-rating--${size}`} aria-label={`Rating promedio ${roundedRating} sobre 5`}>
      <div className="star-rating__stars">{stars}</div>
      <span className="star-rating__value">{roundedRating}</span>
      {typeof reviewsCount === 'number' ? <span className="star-rating__count">({reviewsCount})</span> : null}
    </div>
  )
}

export const StarRating = memo(StarRatingComponent)
