import { Link, useLocation } from 'react-router-dom'
import { useSectionNavigation } from '../../hooks/useSectionNavigation'

export function Navbar() {
  const location = useLocation()
  const isHome = location.pathname === '/'
  const { onSectionLinkClick } = useSectionNavigation()

  return (
    <header className="navbar">
      <div className="container navbar__content">
        <Link to="/" className="navbar__brand">
          The House Bebidas
        </Link>
        <nav className="navbar__links" aria-label="Navegación principal">
          {isHome ? (
            <>
              <a href="#hero" onClick={onSectionLinkClick}>Inicio</a>
              <a href="#vinos" onClick={onSectionLinkClick}>Vinos</a>
              <a href="#about" onClick={onSectionLinkClick}>About</a>
            </>
          ) : (
            <Link to="/">Inicio</Link>
          )}
          <Link to="/admin/login" className="navbar__admin-link">
            Admin
          </Link>
        </nav>
      </div>
    </header>
  )
}
