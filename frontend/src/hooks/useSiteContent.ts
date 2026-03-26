import { useCallback, useEffect, useMemo, useState } from 'react'
import { getSiteContent, mapSiteContentWithDefaults } from '../services/siteContentService'
import type { SiteContentDto } from '../types/api'

export function useSiteContent() {
  const [content, setContent] = useState<SiteContentDto[]>([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const load = useCallback(async () => {
    setLoading(true)
    setError(null)
    try {
      const response = await getSiteContent()
      setContent(response)
    } catch (requestError) {
      const message =
        requestError instanceof Error ? requestError.message : 'No se pudo cargar contenido institucional.'
      setError(message)
    } finally {
      setLoading(false)
    }
  }, [])

  useEffect(() => {
    void load()
  }, [load])

  const contentByKey = useMemo(() => mapSiteContentWithDefaults(content), [content])

  return {
    content,
    contentByKey,
    loading,
    error,
    reload: load,
  }
}
