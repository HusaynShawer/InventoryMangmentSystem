import axios, { AxiosError } from 'axios'
import { clearAuthStorage, getToken } from '@/lib/storage'

// One axios instance for the whole app. It talks to relative `/api` URLs so the
// same code works in dev (Vite proxy) and prod (nginx proxy) with no CORS.
export const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL ?? '/api',
  headers: { 'Content-Type': 'application/json' },
})

api.interceptors.request.use((config) => {
  const token = getToken()
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

api.interceptors.response.use(
  (response) => response,
  (error: AxiosError) => {
    // Expired/invalid token -> drop credentials and bounce to login once.
    if (error.response?.status === 401 && !error.config?.url?.includes('/Auth/login')) {
      clearAuthStorage()
      if (!window.location.pathname.startsWith('/login')) {
        window.location.assign('/login')
      }
    }
    return Promise.reject(error)
  },
)

/**
 * Some list endpoints answer 404 when the collection is empty instead of an
 * empty array. Normalise that to `[]` so the UI can render an empty state.
 */
export async function getList<T>(url: string, params?: Record<string, unknown>): Promise<T[]> {
  try {
    const { data } = await api.get<T[]>(url, { params })
    return Array.isArray(data) ? data : []
  } catch (error) {
    if (axios.isAxiosError(error) && error.response?.status === 404) return []
    throw error
  }
}

/** Turn any API/network failure into a message we can show in a toast. */
export function extractErrorMessage(error: unknown): string {
  if (axios.isAxiosError(error)) {
    const data = error.response?.data
    if (typeof data === 'string' && data.trim()) return data
    if (data && typeof data === 'object') {
      if ('message' in data && typeof (data as { message: unknown }).message === 'string') {
        return (data as { message: string }).message
      }
      // ASP.NET ModelState validation errors: { Field: ["error", ...] }
      const firstEntry = Object.values(data as Record<string, unknown>).find(
        (value) => Array.isArray(value) && value.length > 0,
      )
      if (Array.isArray(firstEntry)) return String(firstEntry[0])
    }
    if (error.response?.status === 401) return 'Your session expired. Please sign in again.'
    if (error.response?.status === 403) return 'You do not have permission to do that.'
    if (!error.response) return 'Cannot reach the server. Is the API running?'
    return error.message
  }
  if (error instanceof Error) return error.message
  return 'Something went wrong.'
}
