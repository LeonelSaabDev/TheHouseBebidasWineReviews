import { useCallback, useEffect, useMemo, useRef, useState } from 'react'
import { useWines } from '../../hooks/useWines'
import { SearchAndSortBar, type SortOption } from './SearchAndSortBar'
import { WineCard } from './WineCard'
import { WineDetailModal } from './WineDetailModal'

function toWineQuery(sortOption: SortOption, searchTerm: string) {
  if (sortOption === 'name-asc') {
    return {
      searchTerm,
      sortBy: 'name' as const,
      sortDescending: false,
    }
  }

  if (sortOption === 'year-desc') {
    return {
      searchTerm,
      sortBy: 'year' as const,
      sortDescending: true,
    }
  }

  return {
    searchTerm,
    sortBy: 'rating' as const,
    sortDescending: true,
  }
}

const SKELETON_CARD_KEYS = ['s1', 's2', 's3', 's4', 's5', 's6']

export function WinesCatalogSection() {
  const pageSize = 20
  const sectionRef = useRef<HTMLElement | null>(null)
  const shouldScrollToTopRef = useRef(false)
  const [searchTerm, setSearchTerm] = useState('')
  const [sortOption, setSortOption] = useState<SortOption>('top-rated')
  const [page, setPage] = useState(1)
  const [selectedWineId, setSelectedWineId] = useState<string | null>(null)

  const wineQuery = useMemo(
    () => ({
      ...toWineQuery(sortOption, searchTerm.trim()),
      page,
      pageSize,
    }),
    [page, pageSize, searchTerm, sortOption],
  )
  const closeWineDetailModal = useCallback(() => setSelectedWineId(null), [])
  const scrollSectionIntoView = useCallback(() => {
    const section = sectionRef.current ?? document.getElementById('vinos')
    if (!section) {
      return
    }

    const navbar = document.querySelector('.navbar') as HTMLElement | null
    const offset = (navbar?.offsetHeight ?? 0) + 12
    const targetTop = section.offsetTop - offset

    window.scrollTo({
      top: Math.max(targetTop, 0),
      behavior: 'smooth',
    })
  }, [])

  const handleSearchChange = useCallback((value: string) => {
    setSearchTerm(value)
    setPage(1)
    scrollSectionIntoView()
  }, [scrollSectionIntoView])

  const handleSortChange = useCallback((value: SortOption) => {
    setSortOption(value)
    setPage(1)
    scrollSectionIntoView()
  }, [scrollSectionIntoView])

  const handlePageChange = useCallback((nextPage: number) => {
    if (nextPage === page) {
      return
    }

    shouldScrollToTopRef.current = true
    setPage(nextPage)
    requestAnimationFrame(scrollSectionIntoView)
  }, [page, scrollSectionIntoView])

  const {
    wines,
    pagination,
    loading: winesLoading,
    isFetching: winesIsFetching,
    error: winesError,
    reload: reloadWines,
  } = useWines(wineQuery)

  useEffect(() => {
    if (!shouldScrollToTopRef.current || winesLoading || winesIsFetching) {
      return
    }

    shouldScrollToTopRef.current = false
    scrollSectionIntoView()
  }, [pagination.page, scrollSectionIntoView, winesIsFetching, winesLoading])

  return (
    <section id="vinos" ref={sectionRef} className="container wines-section">
      <div className="section-heading">
        <p className="eyebrow">Catálogo</p>
        <h2>Vinos</h2>
      </div>

      <SearchAndSortBar
        searchTerm={searchTerm}
        sortOption={sortOption}
        onSearchChange={handleSearchChange}
        onSortChange={handleSortChange}
      />

      {winesLoading ? (
        <div className="wines-grid" aria-hidden="true">
          {SKELETON_CARD_KEYS.map((skeletonKey) => (
            <article key={skeletonKey} className="wine-card-skeleton">
              <div className="wine-card-skeleton__image" />
              <div className="wine-card-skeleton__body">
                <div className="wine-card-skeleton__line wine-card-skeleton__line--short" />
                <div className="wine-card-skeleton__line" />
                <div className="wine-card-skeleton__line wine-card-skeleton__line--soft" />
                <div className="wine-card-skeleton__line wine-card-skeleton__line--soft" />
              </div>
            </article>
          ))}
        </div>
      ) : null}

      {!winesLoading && winesIsFetching ? <p className="state">Actualizando vinos...</p> : null}
      {winesError ? <p className="state state--error">{winesError}</p> : null}

      {!winesError && wines.length > 0 ? (
        <div className="wines-grid">
          {wines.map((wine, index) => (
            <WineCard key={wine.id} wine={wine} onSelect={setSelectedWineId} prioritizeImage={index < 4} />
          ))}
        </div>
      ) : null}

      {!winesLoading && !winesError && wines.length === 0 ? (
        <p className="state">No encontramos vinos con ese criterio.</p>
      ) : null}

      {!winesError && pagination.totalPages > 1 ? (
        <div className="catalog-pagination" aria-label="Paginacion de vinos">
          <button
            type="button"
            className="button button--ghost"
            onClick={() => handlePageChange(Math.max(1, pagination.page - 1))}
            disabled={winesLoading || winesIsFetching || pagination.page <= 1}
          >
            Anterior
          </button>
          <p className="catalog-pagination__status">
            Pagina {pagination.page} de {pagination.totalPages}
          </p>
          <button
            type="button"
            className="button button--ghost"
            onClick={() => handlePageChange(Math.min(pagination.totalPages, pagination.page + 1))}
            disabled={winesLoading || winesIsFetching || pagination.page >= pagination.totalPages}
          >
            Siguiente
          </button>
        </div>
      ) : null}

      {selectedWineId ? (
        <WineDetailModal wineId={selectedWineId} onClose={closeWineDetailModal} onReviewCreated={reloadWines} />
      ) : null}
    </section>
  )
}
