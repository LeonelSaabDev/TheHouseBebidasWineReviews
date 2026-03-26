import { useCallback, useEffect, useRef, useState } from 'react'
import { getWines } from '../services/winesService'
import type { PaginatedResponseDto, WineListItemDto, WineListQuery } from '../types/api'

const EMPTY_PAGINATED_RESPONSE: PaginatedResponseDto<WineListItemDto> = {
  items: [],
  totalItems: 0,
  totalPages: 1,
  page: 1,
  pageSize: 20,
}

export function useWines(query: WineListQuery) {
  const [result, setResult] = useState<PaginatedResponseDto<WineListItemDto>>(EMPTY_PAGINATED_RESPONSE)
  const [loading, setLoading] = useState(false)
  const [isFetching, setIsFetching] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const hasLoadedRef = useRef(false)
  const latestRequestRef = useRef(0)

  const load = useCallback(async (nextQuery: WineListQuery, signal?: AbortSignal) => {
    const requestId = latestRequestRef.current + 1
    latestRequestRef.current = requestId

    if (!hasLoadedRef.current) {
      setLoading(true)
    }

    setIsFetching(true)
    setError(null)

    try {
      const response = await getWines(nextQuery, signal)

      if (latestRequestRef.current === requestId) {
        setResult(response)
        hasLoadedRef.current = true
      }
    } catch (requestError) {
      if (requestError instanceof DOMException && requestError.name === 'AbortError') {
        return
      }

      if (latestRequestRef.current !== requestId) {
        return
      }

      const message =
        requestError instanceof Error ? requestError.message : 'No se pudo cargar el listado de vinos.'
      setError(message)
    } finally {
      if (latestRequestRef.current === requestId) {
        setLoading(false)
        setIsFetching(false)
      }
    }
  }, [])

  useEffect(() => {
    const controller = new AbortController()
    void load(query, controller.signal)

    return () => {
      controller.abort()
    }
  }, [load, query])

  const reload = useCallback(async () => {
    await load(query)
  }, [load, query])

  return {
    wines: result.items,
    pagination: {
      totalItems: result.totalItems,
      totalPages: result.totalPages,
      page: result.page,
      pageSize: result.pageSize,
    },
    loading,
    isFetching,
    error,
    reload,
  }
}
