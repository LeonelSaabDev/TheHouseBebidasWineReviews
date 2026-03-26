import { useCallback, useEffect, useMemo, useState } from 'react'
import type { WineListItemDto } from '../../../types/api'
import { getWines } from '../../../services/winesService'
import {
  createAdminWine,
  deleteAdminWine,
  getAdminWineDetail,
  updateAdminWine,
} from '../services/adminWinesService'
import type { AdminWinePayload } from '../types'
import { normalizeWineImageUrl } from '../../../utils/normalizeImageUrl'

const INITIAL_FORM_STATE: AdminWinePayload = {
  name: '',
  winery: '',
  year: new Date().getFullYear(),
  grapeVariety: '',
  description: '',
  imageUrl: '',
  secondaryImageUrl: null,
  featuredReviewSummary: null,
  isActive: true,
}

export function AdminWinesManager() {
  const [wines, setWines] = useState<WineListItemDto[]>([])
  const [formData, setFormData] = useState<AdminWinePayload>(INITIAL_FORM_STATE)
  const [editingWineId, setEditingWineId] = useState<string | null>(null)
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [processingWineId, setProcessingWineId] = useState<string | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState<string | null>(null)
  const [searchTerm, setSearchTerm] = useState('')

  const filteredWines = useMemo(() => {
    const normalizedSearchTerm = searchTerm.trim().toLowerCase()

    if (!normalizedSearchTerm) {
      return wines
    }

    return wines.filter((wine) => {
      const searchable = `${wine.name} ${wine.winery} ${wine.grapeVariety} ${wine.year}`.toLowerCase()
      return searchable.includes(normalizedSearchTerm)
    })
  }, [searchTerm, wines])

  const loadWines = useCallback(async () => {
    setLoading(true)
    setError(null)

    try {
      const data = await getWines({ sortBy: 'name', page: 1, pageSize: 100 })
      setWines(data.items)
    } catch (requestError) {
      setError(requestError instanceof Error ? requestError.message : 'No se pudo cargar vinos')
    } finally {
      setLoading(false)
    }
  }, [])

  useEffect(() => {
    void loadWines()
  }, [loadWines])

  const clearMessages = () => {
    setError(null)
    setSuccess(null)
  }

  const handleInputChange = (field: keyof AdminWinePayload, value: string | number | boolean | null) => {
    setFormData((current) => ({
      ...current,
      [field]: value,
    }))
  }

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    clearMessages()
    setSaving(true)

    const payload: AdminWinePayload = {
      ...formData,
      year: Number(formData.year),
      imageUrl: normalizeWineImageUrl(formData.imageUrl),
      secondaryImageUrl: formData.secondaryImageUrl?.trim()
        ? normalizeWineImageUrl(formData.secondaryImageUrl)
        : null,
      featuredReviewSummary: formData.featuredReviewSummary?.trim() || null,
    }

    try {
      if (editingWineId) {
        await updateAdminWine(editingWineId, payload)
        setSuccess('Vino actualizado')
      } else {
        await createAdminWine(payload)
        setSuccess('Vino creado')
      }

      setFormData(INITIAL_FORM_STATE)
      setEditingWineId(null)
      await loadWines()
    } catch (requestError) {
      setError(requestError instanceof Error ? requestError.message : 'No se pudo guardar vino')
    } finally {
      setSaving(false)
    }
  }

  const handleEdit = async (wineId: string) => {
    clearMessages()
    setProcessingWineId(wineId)

    try {
      const wine = await getAdminWineDetail(wineId)
      setEditingWineId(wine.id)
      setFormData({
        name: wine.name,
        winery: wine.winery,
        year: wine.year,
        grapeVariety: wine.grapeVariety,
        description: wine.description,
        imageUrl: wine.imageUrl,
        secondaryImageUrl: wine.secondaryImageUrl,
        featuredReviewSummary: wine.featuredReviewSummary,
        isActive: wine.isActive,
      })
    } catch (requestError) {
      setError(requestError instanceof Error ? requestError.message : 'No se pudo cargar detalle del vino')
    } finally {
      setProcessingWineId(null)
    }
  }

  const handleDeactivate = async (wineId: string) => {
    const wine = wines.find((item) => item.id === wineId)
    const shouldDeactivate = window.confirm(
      `Vas a desactivar${wine ? ` ${wine.name}` : ' este vino'}. El vino dejará de estar activo para operaciones administrativas.`,
    )

    if (!shouldDeactivate) {
      return
    }

    clearMessages()
    setProcessingWineId(wineId)

    try {
      const wineDetail = await getAdminWineDetail(wineId)

      if (!wineDetail.isActive) {
        setSuccess('El vino ya estaba desactivado')
        return
      }

      await updateAdminWine(wineId, {
        name: wineDetail.name,
        winery: wineDetail.winery,
        year: wineDetail.year,
        grapeVariety: wineDetail.grapeVariety,
        description: wineDetail.description,
        imageUrl: wineDetail.imageUrl,
        secondaryImageUrl: wineDetail.secondaryImageUrl,
        featuredReviewSummary: wineDetail.featuredReviewSummary,
        isActive: false,
      })

      setSuccess('Vino desactivado')
      await loadWines()
    } catch (requestError) {
      setError(requestError instanceof Error ? requestError.message : 'No se pudo desactivar vino')
    } finally {
      setProcessingWineId(null)
    }
  }

  const handleDelete = async (wineId: string) => {
    const wine = wines.find((item) => item.id === wineId)
    const shouldDelete = window.confirm(
      `Vas a eliminar${wine ? ` ${wine.name}` : ' este vino'}. Esta acción es irreversible y puede afectar referencias de reseñas.`,
    )

    if (!shouldDelete) {
      return
    }

    clearMessages()
    setProcessingWineId(wineId)

    try {
      await deleteAdminWine(wineId)
      setSuccess('Vino eliminado')
      await loadWines()
    } catch (requestError) {
      setError(requestError instanceof Error ? requestError.message : 'No se pudo eliminar vino')
    } finally {
      setProcessingWineId(null)
    }
  }

  const handleCancelEdit = () => {
    setEditingWineId(null)
    setFormData(INITIAL_FORM_STATE)
    clearMessages()
  }

  return (
    <section className="admin-block">
      <div className="admin-block__heading">
        <h2>Vinos</h2>
        <p>CRUD admin de vinos con create, update, deactivate y delete.</p>
      </div>

      <form className="admin-form" onSubmit={handleSubmit}>
        <div className="admin-grid admin-grid--two">
          <label className="field-group">
            <span>Nombre</span>
            <input
              value={formData.name}
              onChange={(event) => handleInputChange('name', event.target.value)}
              required
            />
          </label>
          <label className="field-group">
            <span>Bodega</span>
            <input
              value={formData.winery}
              onChange={(event) => handleInputChange('winery', event.target.value)}
              required
            />
          </label>
          <label className="field-group">
            <span>Año</span>
            <input
              type="number"
              value={formData.year}
              onChange={(event) => handleInputChange('year', Number(event.target.value))}
              required
            />
          </label>
          <label className="field-group">
            <span>Variedad</span>
            <input
              value={formData.grapeVariety}
              onChange={(event) => handleInputChange('grapeVariety', event.target.value)}
              required
            />
          </label>
        </div>

        <label className="field-group">
          <span>Imagen URL</span>
          <input
            value={formData.imageUrl}
            onChange={(event) => handleInputChange('imageUrl', event.target.value)}
            required
          />
        </label>

        <label className="field-group">
          <span>Imagen secundaria URL</span>
          <input
            value={formData.secondaryImageUrl ?? ''}
            onChange={(event) => handleInputChange('secondaryImageUrl', event.target.value || null)}
          />
        </label>

        <label className="field-group">
          <span>Descripción</span>
          <textarea
            value={formData.description}
            onChange={(event) => handleInputChange('description', event.target.value)}
            required
          />
        </label>

        <label className="field-group">
          <span>Featured review (opcional)</span>
          <input
            value={formData.featuredReviewSummary ?? ''}
            onChange={(event) => handleInputChange('featuredReviewSummary', event.target.value)}
          />
        </label>

        <label className="admin-inline-check">
          <input
            type="checkbox"
            checked={formData.isActive}
            onChange={(event) => handleInputChange('isActive', event.target.checked)}
          />
          <span>Activo</span>
        </label>

        <div className="admin-actions">
          <button className="button" type="submit" disabled={saving}>
            {saving ? 'Guardando...' : editingWineId ? 'Actualizar vino' : 'Crear vino'}
          </button>
          {editingWineId ? (
            <button className="button button--ghost" type="button" onClick={handleCancelEdit} disabled={saving}>
              Cancelar edición
            </button>
          ) : null}
        </div>
      </form>

      {error ? <p className="state state--error">{error}</p> : null}
      {success ? <p className="state state--success">{success}</p> : null}

      <label className="field-group">
        <span>Buscar vinos</span>
        <input
          type="search"
          value={searchTerm}
          onChange={(event) => setSearchTerm(event.target.value)}
          placeholder="Nombre, bodega, variedad o año"
        />
      </label>

      {loading ? <p className="state">Cargando vinos...</p> : null}
      {!loading ? <p className="state">Mostrando {filteredWines.length} de {wines.length} vinos.</p> : null}

      {!loading ? (
        <div className="admin-table-wrap">
          <table className="admin-table">
            <thead>
              <tr>
                <th>Nombre</th>
                <th>Bodega</th>
                <th>Año</th>
                <th>Rating</th>
                <th>Acciones</th>
              </tr>
            </thead>
            <tbody>
              {filteredWines.map((wine) => {
                const busy = processingWineId === wine.id

                return (
                  <tr key={wine.id}>
                    <td>{wine.name}</td>
                    <td>{wine.winery}</td>
                    <td>{wine.year}</td>
                    <td>{wine.averageRating.toFixed(1)}</td>
                    <td>
                      <div className="admin-row-actions">
                        <button
                          className="button button--small"
                          type="button"
                          onClick={() => void handleEdit(wine.id)}
                          disabled={busy || saving}
                        >
                          Editar
                        </button>
                        <button
                          className="button button--small button--ghost"
                          type="button"
                          onClick={() => void handleDeactivate(wine.id)}
                          disabled={busy || saving}
                        >
                          Desactivar
                        </button>
                        <button
                          className="button button--small button--danger"
                          type="button"
                          onClick={() => void handleDelete(wine.id)}
                          disabled={busy || saving}
                        >
                          Eliminar
                        </button>
                      </div>
                    </td>
                  </tr>
                )
              })}

              {!filteredWines.length ? (
                <tr>
                  <td colSpan={5}>No hay vinos que coincidan con la búsqueda aplicada.</td>
                </tr>
              ) : null}
            </tbody>
          </table>
        </div>
      ) : null}
    </section>
  )
}
