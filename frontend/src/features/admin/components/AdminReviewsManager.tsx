import { useCallback, useEffect, useMemo, useState } from 'react'
import { deleteAdminReview, getAdminReviews } from '../services/adminReviewsService'
import type { AdminReviewItem } from '../types'

export function AdminReviewsManager() {
  const [reviews, setReviews] = useState<AdminReviewItem[]>([])
  const [loading, setLoading] = useState(true)
  const [processingReviewId, setProcessingReviewId] = useState<string | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState<string | null>(null)
  const [ratingFilter, setRatingFilter] = useState<string>('all')
  const [textFilter, setTextFilter] = useState('')

  const filteredReviews = useMemo(() => {
    const normalizedTextFilter = textFilter.trim().toLowerCase()

    return reviews.filter((review) => {
      const matchesRating = ratingFilter === 'all' ? true : review.rating === Number(ratingFilter)
      const matchesText =
        normalizedTextFilter.length === 0
          ? true
          : `${review.comment ?? ''} ${review.wineId}`.toLowerCase().includes(normalizedTextFilter)

      return matchesRating && matchesText
    })
  }, [ratingFilter, reviews, textFilter])

  const loadReviews = useCallback(async () => {
    setLoading(true)
    setError(null)

    try {
      const data = await getAdminReviews()
      setReviews(data)
    } catch (requestError) {
      setError(requestError instanceof Error ? requestError.message : 'No se pudo cargar reseñas')
    } finally {
      setLoading(false)
    }
  }, [])

  useEffect(() => {
    void loadReviews()
  }, [loadReviews])

  const handleDelete = async (reviewId: string) => {
    const review = reviews.find((item) => item.id === reviewId)
    const shouldDelete = window.confirm(
      `Vas a eliminar esta reseña${review?.comment ? `: "${review.comment.slice(0, 80)}"` : ''}. Esta acción es irreversible.`,
    )

    if (!shouldDelete) {
      return
    }

    setError(null)
    setSuccess(null)
    setProcessingReviewId(reviewId)

    try {
      await deleteAdminReview(reviewId)
      setSuccess('Reseña eliminada')
      await loadReviews()
    } catch (requestError) {
      setError(requestError instanceof Error ? requestError.message : 'No se pudo eliminar reseña')
    } finally {
      setProcessingReviewId(null)
    }
  }

  return (
    <section className="admin-block">
      <div className="admin-block__heading">
        <h2>Reseñas</h2>
        <p>Listado de reseñas admin con eliminación directa.</p>
      </div>

      <div className="admin-grid admin-grid--two">
        <label className="field-group">
          <span>Filtro por rating</span>
          <select value={ratingFilter} onChange={(event) => setRatingFilter(event.target.value)}>
            <option value="all">Todos</option>
            <option value="5">5</option>
            <option value="4">4</option>
            <option value="3">3</option>
            <option value="2">2</option>
            <option value="1">1</option>
          </select>
        </label>

        <label className="field-group">
          <span>Buscar texto</span>
          <input
            type="search"
            value={textFilter}
            onChange={(event) => setTextFilter(event.target.value)}
            placeholder="Comentario o id del vino"
          />
        </label>
      </div>

      {error ? <p className="state state--error">{error}</p> : null}
      {success ? <p className="state state--success">{success}</p> : null}
      {loading ? <p className="state">Cargando reseñas...</p> : null}
      {!loading ? <p className="state">Mostrando {filteredReviews.length} de {reviews.length} reseñas.</p> : null}

      {!loading ? (
        <div className="admin-table-wrap">
          <table className="admin-table">
            <thead>
              <tr>
                <th>Vino</th>
                <th>Rating</th>
                <th>Comentario</th>
                <th>Fecha</th>
                <th>Acciones</th>
              </tr>
            </thead>
            <tbody>
              {filteredReviews.map((review) => {
                const busy = processingReviewId === review.id

                return (
                  <tr key={review.id}>
                    <td>{review.wineId}</td>
                    <td>{review.rating}</td>
                    <td>{review.comment}</td>
                    <td>{new Date(review.createdAt).toLocaleString()}</td>
                    <td>
                      <button
                        className="button button--small button--danger"
                        type="button"
                        onClick={() => void handleDelete(review.id)}
                        disabled={busy}
                      >
                        Eliminar
                      </button>
                    </td>
                  </tr>
                )
              })}

              {!filteredReviews.length ? (
                <tr>
                  <td colSpan={5}>No hay reseñas que coincidan con los filtros aplicados.</td>
                </tr>
              ) : null}
            </tbody>
          </table>
        </div>
      ) : null}
    </section>
  )
}
