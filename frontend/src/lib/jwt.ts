import type { AuthUser, Role } from '@/types'

interface JwtPayload {
  exp?: number
  [key: string]: unknown
}

// .NET's ClaimTypes map to these long URIs inside the JWT payload.
const NAME_IDENTIFIER =
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'
const NAME = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'
const ROLE = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'

/** Decode a JWT payload without verifying the signature (it is verified server-side). */
export function decodeToken(token: string): JwtPayload | null {
  try {
    const [, payload] = token.split('.')
    const decoded = atob(payload.replace(/-/g, '+').replace(/_/g, '/'))
    return JSON.parse(decoded) as JwtPayload
  } catch {
    return null
  }
}

export function isTokenExpired(token: string): boolean {
  const payload = decodeToken(token)
  if (!payload?.exp) return false
  return payload.exp * 1000 <= Date.now()
}

/** Build the app level AuthUser from token claims + login response metadata. */
export function userFromToken(
  token: string,
  fallback?: { userName?: string; role?: string },
): AuthUser {
  const payload = decodeToken(token) ?? {}
  const role = (payload[ROLE] as string) ?? fallback?.role ?? 'User'
  return {
    userName:
      (payload[NAME] as string) ??
      (payload.unique_name as string) ??
      fallback?.userName ??
      'user',
    role: role as Role,
    userId: (payload[NAME_IDENTIFIER] as string) ?? null,
    supplierId: (payload['SupplierId'] as string) ?? null,
    expiresAt: payload.exp ? payload.exp * 1000 : null,
  }
}
