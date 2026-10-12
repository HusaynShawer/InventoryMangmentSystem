import { TOKEN_STORAGE_KEY, USER_STORAGE_KEY } from './constants'
import type { AuthUser } from '@/types'

// Centralised access to persisted auth data. Keeping it in one place makes it
// easy to later swap localStorage for a more XSS resistant strategy.
export function getToken(): string | null {
  return localStorage.getItem(TOKEN_STORAGE_KEY)
}

export function setToken(token: string) {
  localStorage.setItem(TOKEN_STORAGE_KEY, token)
}

export function getStoredUser(): AuthUser | null {
  const raw = localStorage.getItem(USER_STORAGE_KEY)
  if (!raw) return null
  try {
    return JSON.parse(raw) as AuthUser
  } catch {
    return null
  }
}

export function setStoredUser(user: AuthUser) {
  localStorage.setItem(USER_STORAGE_KEY, JSON.stringify(user))
}

export function clearAuthStorage() {
  localStorage.removeItem(TOKEN_STORAGE_KEY)
  localStorage.removeItem(USER_STORAGE_KEY)
}
