import { useEffect } from 'react'

const REVEAL_SELECTOR = '[data-reveal]'

export function useScrollReveal() {
  useEffect(() => {
    const targets = Array.from(document.querySelectorAll<HTMLElement>(REVEAL_SELECTOR))

    if (targets.length === 0) {
      return
    }

    if (typeof IntersectionObserver === 'undefined') {
      for (const target of targets) {
        target.classList.add('is-visible')
      }

      return
    }

    const reducedMotionQuery = window.matchMedia('(prefers-reduced-motion: reduce)')

    if (reducedMotionQuery.matches) {
      for (const target of targets) {
        target.classList.add('is-visible')
      }

      return
    }

    let frameId: number | null = null

    const observer = new IntersectionObserver(
      (entries) => {
        const visibleTargets: HTMLElement[] = []

        for (const entry of entries) {
          if (!entry.isIntersecting) {
            continue
          }

          observer.unobserve(entry.target)

          const target = entry.target as HTMLElement

          if (!target.classList.contains('is-visible')) {
            visibleTargets.push(target)
          }
        }

        if (visibleTargets.length === 0) {
          return
        }

        if (frameId !== null) {
          cancelAnimationFrame(frameId)
        }

        frameId = window.requestAnimationFrame(() => {
          for (const target of visibleTargets) {
            target.classList.add('is-visible')
          }

          frameId = null
        })
      },
      {
        threshold: 0.01,
        rootMargin: '0px 0px -4% 0px',
      },
    )

    const viewportHeight = window.innerHeight || document.documentElement.clientHeight

    for (const target of targets) {
      if (target.classList.contains('is-visible')) {
        continue
      }

      const targetTop = target.getBoundingClientRect().top

      if (targetTop <= viewportHeight * 0.92) {
        target.classList.add('is-visible')
        continue
      }

      observer.observe(target)
    }

    return () => {
      if (frameId !== null) {
        cancelAnimationFrame(frameId)
      }

      observer.disconnect()
    }
  }, [])
}
