import { useCallback, useEffect, useMemo, useState } from 'react'
import { createWineReview, getWineReviews } from '../../services/reviewsService'
import { getWineDetail } from '../../services/winesService'
import type { ReviewDto, WineDetailDto } from '../../types/api'
import { getWineImageCandidates } from '../../utils/normalizeImageUrl'
import { StarRating } from './StarRating'

interface ReviewFormErrors {
  authorName?: string
  comment?: string
  rating?: string
}

interface WineDetailModalProps {
  wineId: string
  onClose: () => void
  onReviewCreated: () => void
}

const COMMENT_MAX = 400
const AUTHOR_NAME_MAX = 80

export function WineDetailModal({ wineId, onClose, onReviewCreated }: WineDetailModalProps) {
  const [wine, setWine] = useState<WineDetailDto | null>(null)
  const [reviews, setReviews] = useState<ReviewDto[]>([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [authorName, setAuthorName] = useState('')
  const [comment, setComment] = useState('')
  const [rating, setRating] = useState<number | ''>('')
  const [formErrors, setFormErrors] = useState<ReviewFormErrors>({})
  const [submitError, setSubmitError] = useState<string | null>(null)
  const [submitSuccess, setSubmitSuccess] = useState<string | null>(null)
  const [submitting, setSubmitting] = useState(false)
  const [selectedImage, setSelectedImage] = useState<'primary' | 'secondary'>('primary')
  const [activeImageCandidateIndex, setActiveImageCandidateIndex] = useState(0)
  const [thumbnailCandidateIndex, setThumbnailCandidateIndex] = useState<Record<'primary' | 'secondary', number>>({
    primary: 0,
    secondary: 0,
  })

  const loadWineAndReviews = useCallback(
    async () => {
      const [wineResponse, reviewResponse] = await Promise.all([
        getWineDetail(wineId),
        getWineReviews(wineId),
      ])

      return {
        wine: wineResponse,
        reviews: reviewResponse,
      }
    },
    [wineId],
  )

  useEffect(() => {
    const onEscape = (event: KeyboardEvent) => {
      if (event.key === 'Escape') {
        onClose()
      }
    }

    document.addEventListener('keydown', onEscape)
    return () => document.removeEventListener('keydown', onEscape)
  }, [onClose])

  useEffect(() => {
    let isActive = true

    const load = async () => {
      setLoading(true)
      setError(null)

      try {
        const response = await loadWineAndReviews()

        if (!isActive) {
          return
        }

        setWine(response.wine)
        setReviews(response.reviews)
      } catch (requestError) {
        if (!isActive) {
          return
        }

        const message = requestError instanceof Error ? requestError.message : 'No se pudo cargar el detalle del vino.'
        setError(message)
      } finally {
        if (isActive) {
          setLoading(false)
        }
      }
    }

    void load()

    return () => {
      isActive = false
    }
  }, [loadWineAndReviews])

  const remainingCharacters = useMemo(() => COMMENT_MAX - comment.length, [comment.length])
  const imageOptions = useMemo(() => {
    if (!wine) {
      return []
    }

    const images = [
      {
        key: 'primary' as const,
        label: 'Principal',
        candidates: getWineImageCandidates(wine.imageUrl),
      },
    ]

    if (wine.secondaryImageUrl) {
      images.push({
        key: 'secondary' as const,
        label: 'Secundaria',
        candidates: getWineImageCandidates(wine.secondaryImageUrl),
      })
    }

    return images
  }, [wine])
  const activeImage = imageOptions.find((image) => image.key === selectedImage) ?? imageOptions[0]
  const activeImageUrl = activeImage?.candidates[activeImageCandidateIndex] ?? activeImage?.candidates[0] ?? ''
  const isCounterWarning = remainingCharacters <= 60
  const commentErrorId = `review-comment-error-${wineId}`
  const authorNameErrorId = `review-author-name-error-${wineId}`
  const ratingErrorId = `review-rating-error-${wineId}`

  useEffect(() => {
    setSelectedImage('primary')
  }, [wine?.id])

  useEffect(() => {
    setActiveImageCandidateIndex(0)
  }, [selectedImage, wine?.id])

  useEffect(() => {
    setThumbnailCandidateIndex({
      primary: 0,
      secondary: 0,
    })
  }, [wine?.id])

  const validate = () => {
    const nextErrors: ReviewFormErrors = {}
    const normalizedAuthorName = authorName.trim()
    const normalizedComment = comment.trim()

    if (!normalizedAuthorName) {
      nextErrors.authorName = 'El nombre es obligatorio.'
    } else if (normalizedAuthorName.length > AUTHOR_NAME_MAX) {
      nextErrors.authorName = `El nombre no puede superar ${AUTHOR_NAME_MAX} caracteres.`
    }

    if (!normalizedComment) {
      nextErrors.comment = 'El comentario es obligatorio.'
    } else if (normalizedComment.length > COMMENT_MAX) {
      nextErrors.comment = `El comentario no puede superar ${COMMENT_MAX} caracteres.`
    }

    if (rating === '') {
      nextErrors.rating = 'La puntuación es obligatoria.'
    }

    setFormErrors(nextErrors)
    return Object.keys(nextErrors).length === 0
  }

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    setSubmitError(null)
    setSubmitSuccess(null)

    if (!validate()) {
      return
    }

    setSubmitting(true)

    try {
      await createWineReview(wineId, {
        authorName: authorName.trim(),
        comment: comment.trim(),
        rating: Number(rating),
      })

      const response = await loadWineAndReviews()

      setWine(response.wine)
      setReviews(response.reviews)
      setAuthorName('')
      setComment('')
      setRating('')
      setFormErrors({})
      setSubmitSuccess('Reseña publicada correctamente.')
      onReviewCreated()
    } catch (requestError) {
      const message = requestError instanceof Error ? requestError.message : 'No se pudo publicar la reseña.'
      setSubmitError(message)
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <div className="modal-backdrop" role="presentation" onClick={onClose}>
      <section
        className="modal"
        role="dialog"
        aria-modal="true"
        aria-labelledby="wine-detail-title"
        onClick={(event) => event.stopPropagation()}
      >
        <button className="modal__close" onClick={onClose} aria-label="Cerrar detalle de vino">
          x
        </button>

        {loading ? <p>Cargando detalle...</p> : null}
        {error ? <p className="state state--error">{error}</p> : null}

        {!loading && !error && wine ? (
          <>
            <div className="modal__gallery">
              <div className="modal__image-frame">
                <img
                  key={`${activeImage?.key ?? 'primary'}-${activeImageCandidateIndex}`}
                  src={activeImageUrl}
                  alt={`${wine.name} de ${wine.winery}`}
                  className="modal__image modal__image--animated"
                  onError={() => {
                    if (!activeImage) {
                      return
                    }

                    setActiveImageCandidateIndex((current) => Math.min(current + 1, activeImage.candidates.length - 1))
                  }}
                />
              </div>

              {imageOptions.length > 1 ? (
                <div className="modal__gallery-thumbs" role="tablist" aria-label="Galeria de imagenes del vino">
                  {imageOptions.map((image) => {
                    const isActive = image.key === activeImage.key
                    const thumbIndex = thumbnailCandidateIndex[image.key] ?? 0
                    const thumbSrc = image.candidates[thumbIndex] ?? image.candidates[0] ?? ''

                    return (
                      <button
                        key={image.key}
                        type="button"
                        className={`modal__thumb ${isActive ? 'modal__thumb--active' : ''}`}
                        onClick={() => setSelectedImage(image.key)}
                        role="tab"
                        aria-selected={isActive}
                        aria-label={`Mostrar imagen ${image.label.toLowerCase()}`}
                      >
                        <img
                          src={thumbSrc}
                          alt={`${wine.name} - vista ${image.label.toLowerCase()}`}
                          onError={() => {
                            setThumbnailCandidateIndex((current) => ({
                              ...current,
                              [image.key]: Math.min((current[image.key] ?? 0) + 1, image.candidates.length - 1),
                            }))
                          }}
                        />
                      </button>
                    )
                  })}
                </div>
              ) : null}
            </div>

            <div className="modal__header">
              <p className="wine-card__meta">
                {wine.winery} · {wine.year}
              </p>
              <h2 id="wine-detail-title">{wine.name}</h2>
              <p className="wine-card__variety">{wine.grapeVariety}</p>
              <StarRating rating={wine.averageRating} reviewsCount={wine.reviewsCount} />
              <p>{wine.description}</p>
            </div>

            <div className="modal__grid">
              <div className="review-list">
                <h3>Reseñas públicas</h3>
                {reviews.length === 0 ? <p>Todavía no hay reseñas para este vino.</p> : null}
                {reviews.map((review) => (
                  <article key={review.id} className="review-item">
                    <p>
                      <strong>{review.authorName}</strong>
                    </p>
                    <StarRating rating={review.rating} size="sm" />
                    <p>{review.comment}</p>
                    <time dateTime={review.createdAt}>
                      {new Date(review.createdAt).toLocaleDateString('es-AR', {
                        day: '2-digit',
                        month: 'short',
                        year: 'numeric',
                      })}
                    </time>
                  </article>
                ))}
              </div>

              <form className="review-form" onSubmit={handleSubmit}>
                <h3>Dejá tu reseña</h3>
                <label className="field-group">
                  <span>Nombre</span>
                  <input
                    type="text"
                    value={authorName}
                    required
                    maxLength={AUTHOR_NAME_MAX}
                    onChange={(event) => setAuthorName(event.target.value)}
                    placeholder="Tu nombre"
                    aria-invalid={Boolean(formErrors.authorName)}
                    aria-describedby={formErrors.authorName ? authorNameErrorId : undefined}
                  />
                </label>
                {formErrors.authorName ? (
                  <p id={authorNameErrorId} className="state state--error" role="alert">
                    {formErrors.authorName}
                  </p>
                ) : null}
                <label className="field-group">
                  <span>Comentario</span>
                  <textarea
                    value={comment}
                    maxLength={COMMENT_MAX}
                    onChange={(event) => setComment(event.target.value)}
                    placeholder="Contanos tu experiencia con este vino"
                    aria-invalid={Boolean(formErrors.comment)}
                    aria-describedby={formErrors.comment ? commentErrorId : undefined}
                  />
                </label>
                <p className={`helper-text ${isCounterWarning ? 'helper-text--warning' : ''}`}>
                  {remainingCharacters} caracteres disponibles
                </p>
                {formErrors.comment ? (
                  <p id={commentErrorId} className="state state--error" role="alert">
                    {formErrors.comment}
                  </p>
                ) : null}

                <label className="field-group">
                  <span>Puntuación</span>
                  <select
                    value={rating}
                    onChange={(event) => setRating(event.target.value ? Number(event.target.value) : '')}
                    aria-invalid={Boolean(formErrors.rating)}
                    aria-describedby={formErrors.rating ? ratingErrorId : undefined}
                  >
                    <option value="">Seleccionar rating</option>
                    <option value="1">1 estrella</option>
                    <option value="2">2 estrellas</option>
                    <option value="3">3 estrellas</option>
                    <option value="4">4 estrellas</option>
                    <option value="5">5 estrellas</option>
                  </select>
                </label>
                {formErrors.rating ? (
                  <p id={ratingErrorId} className="state state--error" role="alert">
                    {formErrors.rating}
                  </p>
                ) : null}

                {submitError ? <p className="state state--error">{submitError}</p> : null}
                {submitSuccess ? <p className="state state--success">{submitSuccess}</p> : null}

                <button className="button button--full" type="submit" disabled={submitting}>
                  {submitting ? 'Publicando...' : 'Publicar reseña'}
                </button>
              </form>
            </div>
          </>
        ) : null}
      </section>
    </div>
  )
}
