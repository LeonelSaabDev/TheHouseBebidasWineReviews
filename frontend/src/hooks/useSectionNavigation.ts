import { useCallback, type MouseEventHandler } from 'react'

const SECTION_PULSE_CLASS = 'section-focus-pulse'
const SECTION_PULSE_DURATION_MS = 360
const activePulseTimers = new WeakMap<HTMLElement, number>()

function prefersReducedMotion() {
  return window.matchMedia('(prefers-reduced-motion: reduce)').matches
}

function pulseSection(target: HTMLElement) {
  const previousTimer = activePulseTimers.get(target)
  if (previousTimer) {
    window.clearTimeout(previousTimer)
  }

  target.classList.remove(SECTION_PULSE_CLASS)
  void target.offsetWidth
  target.classList.add(SECTION_PULSE_CLASS)

  const timer = window.setTimeout(() => {
    target.classList.remove(SECTION_PULSE_CLASS)
    activePulseTimers.delete(target)
  }, SECTION_PULSE_DURATION_MS)

  activePulseTimers.set(target, timer)
}

export function useSectionNavigation() {
  const onSectionLinkClick = useCallback<MouseEventHandler<HTMLAnchorElement>>((event) => {
    const sectionHash = event.currentTarget.getAttribute('href')
    if (!sectionHash || !sectionHash.startsWith('#')) {
      return
    }

    const target = document.querySelector<HTMLElement>(sectionHash)
    if (!target) {
      return
    }

    event.preventDefault()

    const reduceMotion = prefersReducedMotion()
    target.scrollIntoView({
      behavior: reduceMotion ? 'auto' : 'smooth',
      block: 'start',
    })

    if (!reduceMotion) {
      pulseSection(target)
    }

    if (window.location.hash !== sectionHash) {
      window.history.replaceState(null, '', sectionHash)
    }
  }, [])

  return { onSectionLinkClick }
}
