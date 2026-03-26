import { memo } from 'react'
import { useSectionNavigation } from '../../hooks/useSectionNavigation'

export const Footer = memo(function Footer() {
  const { onSectionLinkClick } = useSectionNavigation()

  return (
    <footer id="footer" className="footer">
      <div className="container footer__content">
        <div className="footer__main">
          <div className="footer__brand-row">
            <p className="footer__brand">The House Bebidas Wine Reviews</p>
            <div className="footer__social" aria-label="Redes sociales">
              <a
                className="footer__social-button"
                href="https://www.instagram.com/thehousebebidasok/"
                target="_blank"
                rel="noopener noreferrer"
                aria-label="Instagram de The House Bebidas"
              >
                <svg viewBox="0 0 24 24" aria-hidden="true" focusable="false">
                  <path d="M7.75 2h8.5A5.75 5.75 0 0 1 22 7.75v8.5A5.75 5.75 0 0 1 16.25 22h-8.5A5.75 5.75 0 0 1 2 16.25v-8.5A5.75 5.75 0 0 1 7.75 2Zm0 1.5A4.25 4.25 0 0 0 3.5 7.75v8.5a4.25 4.25 0 0 0 4.25 4.25h8.5a4.25 4.25 0 0 0 4.25-4.25v-8.5a4.25 4.25 0 0 0-4.25-4.25h-8.5Zm8.95 1.5a1.05 1.05 0 1 1 0 2.1 1.05 1.05 0 0 1 0-2.1ZM12 7a5 5 0 1 1 0 10 5 5 0 0 1 0-10Zm0 1.5a3.5 3.5 0 1 0 0 7 3.5 3.5 0 0 0 0-7Z" />
                </svg>
              </a>

              <a
                className="footer__social-button"
                href="https://www.facebook.com/profile.php?id=61570883492173"
                target="_blank"
                rel="noopener noreferrer"
                aria-label="Facebook de The House Bebidas"
              >
                <svg viewBox="0 0 24 24" aria-hidden="true" focusable="false">
                  <path d="M13.5 22v-8.8h2.96l.44-3.44H13.5V7.56c0-1 .28-1.68 1.72-1.68h1.84V2.8a24.8 24.8 0 0 0-2.68-.14c-2.64 0-4.45 1.6-4.45 4.56v2.54H7v3.44h2.93V22h3.57Z" />
                </svg>
              </a>
            </div>
          </div>
          <p className="footer__meta">Todos los derechos reservados ©The House Bebidas. Página de reseñas de vinos.</p>
        </div>
        <nav aria-label="Navegación de footer" className="footer__nav">
          <a href="#hero" onClick={onSectionLinkClick}>Inicio</a>
          <a href="#vinos" onClick={onSectionLinkClick}>Vinos</a>
          <a href="#about" onClick={onSectionLinkClick}>About</a>
        </nav>
      </div>
    </footer>
  )
})
