import { useCallback, useEffect, useMemo, useState } from 'react'
import { getSiteContent } from '../../../services/siteContentService'
import { updateAdminSiteContent } from '../services/adminSiteContentService'
import type { AdminSiteContentItem } from '../types'

interface SiteContentFormState {
  title: string
  content: string
}

const SECTION_KEYS = ['hero', 'about', 'footer'] as const

function toFormState(section?: AdminSiteContentItem): SiteContentFormState {
  return {
    title: section?.title ?? '',
    content: section?.content ?? '',
  }
}

export function AdminSiteContentManager() {
  const [sections, setSections] = useState<AdminSiteContentItem[]>([])
  const [forms, setForms] = useState<Record<string, SiteContentFormState>>({})
  const [loading, setLoading] = useState(true)
  const [savingKey, setSavingKey] = useState<string | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState<string | null>(null)

  const trackedSections = useMemo(
    () =>
      SECTION_KEYS.map((key) => ({
        key,
        section: sections.find((item) => item.key === key),
      })),
    [sections],
  )

  const loadSections = useCallback(async () => {
    setLoading(true)
    setError(null)

    try {
      const data = await getSiteContent()
      setSections(data)
      setForms(
        SECTION_KEYS.reduce<Record<string, SiteContentFormState>>((accumulator, key) => {
          const section = data.find((item) => item.key === key)
          accumulator[key] = toFormState(section)
          return accumulator
        }, {}),
      )
    } catch (requestError) {
      setError(requestError instanceof Error ? requestError.message : 'No se pudo cargar contenido')
    } finally {
      setLoading(false)
    }
  }, [])

  useEffect(() => {
    void loadSections()
  }, [loadSections])

  const handleFieldChange = (key: string, field: keyof SiteContentFormState, value: string) => {
    setForms((current) => ({
      ...current,
      [key]: {
        ...current[key],
        [field]: value,
      },
    }))
  }

  const handleSave = async (key: string) => {
    const payload = forms[key]

    if (!payload) {
      return
    }

    setError(null)
    setSuccess(null)
    setSavingKey(key)

    try {
      const updated = await updateAdminSiteContent(key, {
        title: payload.title,
        content: payload.content,
      })

      setSections((current) => {
        const existingIndex = current.findIndex((item) => item.key === key)

        if (existingIndex < 0) {
          return [...current, updated]
        }

        return current.map((item) => (item.key === key ? updated : item))
      })

      setSuccess(`Sección ${key} actualizada`)
    } catch (requestError) {
      setError(requestError instanceof Error ? requestError.message : `No se pudo guardar ${key}`)
    } finally {
      setSavingKey(null)
    }
  }

  return (
    <section className="admin-block">
      <div className="admin-block__heading">
        <h2>Site content</h2>
        <p>Edición por key para hero, about y footer.</p>
      </div>

      {error ? <p className="state state--error">{error}</p> : null}
      {success ? <p className="state state--success">{success}</p> : null}
      {loading ? <p className="state">Cargando contenido...</p> : null}

      {!loading ? (
        <div className="admin-stack">
          {trackedSections.map(({ key, section }) => {
            const form = forms[key] ?? toFormState(section)
            const busy = savingKey === key

            return (
              <article key={key} className="admin-card">
                <div className="admin-card__header">
                  <h3>{key}</h3>
                  <p className="helper-text">
                    Última actualización:{' '}
                    {section ? new Date(section.updatedAt).toLocaleString() : 'Sin registro previo'}
                  </p>
                </div>

                <label className="field-group">
                  <span>Título</span>
                  <input
                    value={form.title}
                    onChange={(event) => handleFieldChange(key, 'title', event.target.value)}
                  />
                </label>

                <label className="field-group">
                  <span>Contenido</span>
                  <textarea
                    value={form.content}
                    onChange={(event) => handleFieldChange(key, 'content', event.target.value)}
                  />
                </label>

                <button className="button" type="button" onClick={() => void handleSave(key)} disabled={busy}>
                  {busy ? 'Guardando...' : `Guardar ${key}`}
                </button>
              </article>
            )
          })}
        </div>
      ) : null}
    </section>
  )
}
