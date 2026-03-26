const DRIVE_DIRECT_BASE = 'https://lh3.googleusercontent.com/d'
const DRIVE_USERCONTENT_BASE = 'https://drive.usercontent.google.com/download'
const DRIVE_THUMBNAIL_BASE = 'https://drive.google.com/thumbnail'
const IMAGE_PROXY_PATH = '/api/images/proxy'
const DRIVE_HOSTS = new Set([
  'drive.google.com',
  'drive.usercontent.google.com',
  'lh3.googleusercontent.com',
])

function getApiBaseUrl(): string {
  const configuredBaseUrl = import.meta.env.VITE_API_BASE_URL?.toString().trim() ?? ''
  return configuredBaseUrl.replace(/\/+$/, '')
}

function buildImageProxyUrl(rawUrl: string): string {
  const baseUrl = getApiBaseUrl()
  const proxyUrl = `${IMAGE_PROXY_PATH}?url=${encodeURIComponent(rawUrl)}`

  if (!baseUrl) {
    return proxyUrl
  }

  return `${baseUrl}${proxyUrl}`
}

function isDriveHostUrl(rawUrl: string): boolean {
  try {
    const parsed = new URL(rawUrl)
    return DRIVE_HOSTS.has(parsed.hostname)
  } catch {
    return false
  }
}

interface DriveUrlParts {
  fileId: string
  resourceKey: string | null
}

function extractGoogleDriveUrlParts(input: string): DriveUrlParts | null {
  const trimmed = input.trim()

  if (!trimmed) {
    return null
  }

  const filePathMatch = trimmed.match(/\/file\/d\/([^/]+)/i)
  if (filePathMatch?.[1]) {
    try {
      const parsed = new URL(trimmed)
      return {
        fileId: filePathMatch[1],
        resourceKey: parsed.searchParams.get('resourcekey'),
      }
    } catch {
      return {
        fileId: filePathMatch[1],
        resourceKey: null,
      }
    }
  }

  try {
    const parsed = new URL(trimmed)
    const idFromParams = parsed.searchParams.get('id')
    if (idFromParams) {
      return {
        fileId: idFromParams,
        resourceKey: parsed.searchParams.get('resourcekey'),
      }
    }

    if (parsed.hostname === 'drive.usercontent.google.com' && parsed.pathname.includes('/download')) {
      const id = parsed.searchParams.get('id')
      if (!id) {
        return null
      }

      return {
        fileId: id,
        resourceKey: parsed.searchParams.get('resourcekey'),
      }
    }
  } catch {
    return null
  }

  return null
}

function appendResourceKey(url: string, resourceKey: string | null): string {
  if (!resourceKey) {
    return url
  }

  return `${url}&resourcekey=${encodeURIComponent(resourceKey)}`
}

export function getWineImageCandidates(rawUrl: string): string[] {
  const trimmed = rawUrl.trim()

  if (!trimmed) {
    return []
  }

  const driveParts = extractGoogleDriveUrlParts(trimmed)
  if (!driveParts && !isDriveHostUrl(trimmed)) {
    return [trimmed]
  }

  const directCandidates = driveParts
    ? [
      appendResourceKey(`${DRIVE_DIRECT_BASE}/${driveParts.fileId}=w2000`, driveParts.resourceKey),
      appendResourceKey(`${DRIVE_USERCONTENT_BASE}?id=${driveParts.fileId}&export=view`, driveParts.resourceKey),
      appendResourceKey(`${DRIVE_THUMBNAIL_BASE}?id=${driveParts.fileId}&sz=w2000`, driveParts.resourceKey),
      trimmed,
    ]
    : [trimmed]

  const proxiedCandidates = directCandidates.map(buildImageProxyUrl)

  return Array.from(new Set([...proxiedCandidates, ...directCandidates]))
}

export function normalizeWineImageUrl(rawUrl: string): string {
  const trimmed = rawUrl.trim()
  const driveParts = extractGoogleDriveUrlParts(trimmed)

  if (!driveParts) {
    return trimmed
  }

  return appendResourceKey(`${DRIVE_DIRECT_BASE}/${driveParts.fileId}=w2000`, driveParts.resourceKey)
}
