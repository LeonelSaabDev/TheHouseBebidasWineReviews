import { Footer } from '../../components/layout/Footer'
import { Navbar } from '../../components/layout/Navbar'
import { WinesCatalogSection } from '../../components/wines/WinesCatalogSection'
import { useScrollReveal } from '../../hooks/useScrollReveal'
import { useSectionNavigation } from '../../hooks/useSectionNavigation'
import { useSiteContent } from '../../hooks/useSiteContent'

export function HomePage() {
  const { onSectionLinkClick } = useSectionNavigation()

  useScrollReveal()

  const { contentByKey, error: contentError } = useSiteContent()

  return (
    <div className="page-shell">
      <Navbar />

      <main>
        <section id="hero" className="hero-section">
          <div className="hero-section__overlay" />
          <div className="container hero-section__content">
            <p className="eyebrow">Wine Reviews</p>
            <h1 className="hero-section__title">{contentByKey.hero.title}</h1>
            <p className="hero-section__subtitle">
              {contentByKey.hero.content}
            </p>
            <div className="hero-section__actions">
              <a className="button" href="#vinos" onClick={onSectionLinkClick}>
                Explorar Vinos
              </a>
              <div className="hero-section__social" aria-label="Redes sociales de The House Bebidas">
                <a
                  className="hero-section__social-button"
                  href="https://www.instagram.com/thehousebebidasok/"
                  target="_blank"
                  rel="noopener noreferrer"
                  aria-label="Instagram de The House Bebidas"
                >
                  <svg viewBox="0 0 24 24" aria-hidden="true" focusable="false">
                    <path d="M7.2 2h9.6A5.2 5.2 0 0 1 22 7.2v9.6a5.2 5.2 0 0 1-5.2 5.2H7.2A5.2 5.2 0 0 1 2 16.8V7.2A5.2 5.2 0 0 1 7.2 2Zm0 1.8A3.4 3.4 0 0 0 3.8 7.2v9.6a3.4 3.4 0 0 0 3.4 3.4h9.6a3.4 3.4 0 0 0 3.4-3.4V7.2a3.4 3.4 0 0 0-3.4-3.4H7.2Zm10.25 1.35a1.2 1.2 0 1 1 0 2.4 1.2 1.2 0 0 1 0-2.4ZM12 7a5 5 0 1 1 0 10 5 5 0 0 1 0-10Zm0 1.8a3.2 3.2 0 1 0 0 6.4 3.2 3.2 0 0 0 0-6.4Z" />
                  </svg>
                  <span>Instagram</span>
                </a>
                <a
                  className="hero-section__social-button"
                  href="https://www.facebook.com/profile.php?id=61570883492173"
                  target="_blank"
                  rel="noopener noreferrer"
                  aria-label="Facebook de The House Bebidas"
                >
                  <svg viewBox="0 0 24 24" aria-hidden="true" focusable="false">
                    <path d="M13.35 22v-8.2h2.75l.42-3.2h-3.17V8.57c0-.93.26-1.56 1.6-1.56H16.7V4.14c-.3-.04-1.34-.14-2.55-.14-2.52 0-4.24 1.53-4.24 4.36v2.24H7.1v3.2h2.8V22h3.45Z" />
                  </svg>
                  <span>Facebook</span>
                </a>
              </div>
            </div>
          </div>
          <img
            src="/images/VinoHero2.jpg"
            alt="Copa de vino sobre mesa"
            className="hero-section__image"
          />
        </section>

        <WinesCatalogSection />

        <section id="about" className="about-section reveal-section" data-reveal>
          <div className="container about-section__content">
            <div className="about-section__grid">
              <div className="about-section__logo-wrap">
                <img src="/images/logo.png" alt="Logo de The House Bebidas" className="about-section__logo" loading="lazy" />
              </div>

              <div className="section-heading section-heading--about">
                <p className="eyebrow">ABOUT</p>
                <h2>The House Bebidas</h2>
                <p className="about-section__lead">
                  En The House Bebidas vivimos el mundo del vino con pasión. Creamos este espacio para compartir
                  reseñas, opiniones y recomendaciones que ayuden a descubrir nuevas etiquetas, conocer mejor cada
                  botella y disfrutar más cada experiencia.
                </p>
                <p className="about-section__text">
                  Nuestra propuesta es simple: acercar el vino a todos, desde quienes recién comienzan a explorarlo
                  hasta quienes ya lo disfrutan y quieren encontrar nuevas opciones. A través de reseñas claras y
                  honestas, buscamos destacar sabores, aromas, estilos y sensaciones que hacen único a cada vino.
                </p>
                <p className="about-section__text">
                  Más que una página, queremos construir una comunidad para descubrir, valorar y hablar del vino de
                  una forma cercana, moderna y auténtica.
                </p>
              </div>
            </div>
            {contentError ? <p className="state state--error">{contentError}</p> : null}
          </div>
        </section>
      </main>

      <Footer />
    </div>
  )
}
