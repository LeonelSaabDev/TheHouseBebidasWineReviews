import { memo, useCallback, type ChangeEvent } from 'react'

type SortOption = 'top-rated' | 'name-asc' | 'year-desc'

interface SearchAndSortBarProps {
  searchTerm: string
  sortOption: SortOption
  onSearchChange: (value: string) => void
  onSortChange: (value: SortOption) => void
}

function SearchAndSortBarComponent({
  searchTerm,
  sortOption,
  onSearchChange,
  onSortChange,
}: SearchAndSortBarProps) {
  const handleSearchChange = useCallback(
    (event: ChangeEvent<HTMLInputElement>) => {
      onSearchChange(event.target.value)
    },
    [onSearchChange],
  )

  const handleSortChange = useCallback(
    (event: ChangeEvent<HTMLSelectElement>) => {
      onSortChange(event.target.value as SortOption)
    },
    [onSortChange],
  )

  return (
    <div className="search-sort-bar">
      <label className="field-group">
        <span>Buscar vino</span>
        <input
          type="search"
          value={searchTerm}
          onChange={handleSearchChange}
          placeholder="Nombre, bodega o variedad..."
          aria-label="Buscar vinos"
        />
      </label>

      <label className="field-group">
        <span>Ordenar</span>
        <select value={sortOption} onChange={handleSortChange}>
          <option value="top-rated">Mejor valorados</option>
          <option value="name-asc">Nombre A-Z</option>
          <option value="year-desc">Últimos añadidos</option>
        </select>
      </label>
    </div>
  )
}

export const SearchAndSortBar = memo(SearchAndSortBarComponent)

export type { SortOption }
