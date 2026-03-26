import type { SiteContentDto } from '../types/api'
import { apiRequest } from './apiClient'

export type SiteContentKey = 'hero' | 'about' | 'footer'

export const DEFAULT_SITE_CONTENT: Readonly<Record<SiteContentKey, { title: string; content: string }>> = {
  hero: {
    title: 'The House Bebidas',
    content: 'Una experiencia moderna para explorar vinos, leer reseñas reales y elegir mejor cada botella.',
  },
  about: {
    title: 'The House Bebidas',
    content:
      'Combinamos selección curada, contexto enológico y recomendaciones de la comunidad para que elijas cada botella con más confianza.',
  },
  footer: {
    title: 'The House Bebidas Wine Reviews',
    content: '',
  },
}

function isSiteContentKey(key: string): key is SiteContentKey {
  return key === 'hero' || key === 'about' || key === 'footer'
}

function toTimestamp(value: string) {
  const parsed = Date.parse(value)
  return Number.isNaN(parsed) ? 0 : parsed
}

export function mapSiteContentWithDefaults(content: SiteContentDto[]) {
  const latestByKey = new Map<SiteContentKey, SiteContentDto>()

  for (const item of content) {
    if (!isSiteContentKey(item.key)) {
      continue
    }

    const current = latestByKey.get(item.key)

    if (!current || toTimestamp(item.updatedAt) >= toTimestamp(current.updatedAt)) {
      latestByKey.set(item.key, item)
    }
  }

  return {
    hero: {
      title: latestByKey.get('hero')?.title || DEFAULT_SITE_CONTENT.hero.title,
      content: latestByKey.get('hero')?.content || DEFAULT_SITE_CONTENT.hero.content,
    },
    about: {
      title: latestByKey.get('about')?.title || DEFAULT_SITE_CONTENT.about.title,
      content: latestByKey.get('about')?.content || DEFAULT_SITE_CONTENT.about.content,
    },
    footer: {
      title: latestByKey.get('footer')?.title || DEFAULT_SITE_CONTENT.footer.title,
      content: latestByKey.get('footer')?.content || DEFAULT_SITE_CONTENT.footer.content,
    },
  }
}

export function getSiteContent() {
  return apiRequest<SiteContentDto[]>('/api/site-content', {
    method: 'GET',
  })
}
