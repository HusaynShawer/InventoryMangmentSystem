import {
  createContext,
  useCallback,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from 'react'
import { loginRequest } from '@/api/auth'
import { clearAuthStorage, getStoredUser, getToken, setStoredUser, setToken } from '@/lib/storage'
import { isTokenExpired, userFromToken } from '@/lib/jwt'
import type { AuthUser, LoginDto, Role } from '@/types'

interface AuthContextValue {
  user: AuthUser | null
  isAuthenticated: boolean
  login: (payload: LoginDto) => Promise<AuthUser>
  logout: () => void
  hasRole: (...roles: Role[]) => boolean
}

export const AuthContext = createContext<AuthContextValue | null>(null)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<AuthUser | null>(() => {
    const token = getToken()
    if (!token || isTokenExpired(token)) {
      clearAuthStorage()
      return null
    }
    return getStoredUser() ?? userFromToken(token)
  })

  const logout = useCallback(() => {
    clearAuthStorage()
    setUser(null)
  }, [])

  const login = useCallback(async (payload: LoginDto) => {
    const response = await loginRequest(payload)
    setToken(response.token)
    const authUser = userFromToken(response.token, {
      userName: response.userName,
      role: response.role,
    })
    setStoredUser(authUser)
    setUser(authUser)
    return authUser
  }, [])

  // Auto logout when the JWT passes its expiry while the tab stays open.
  useEffect(() => {
    if (!user?.expiresAt) return
    const delay = user.expiresAt - Date.now()
    if (delay <= 0) {
      logout()
      return
    }
    const timer = window.setTimeout(logout, delay)
    return () => window.clearTimeout(timer)
  }, [user, logout])

  const value = useMemo<AuthContextValue>(
    () => ({
      user,
      isAuthenticated: Boolean(user),
      login,
      logout,
      hasRole: (...roles: Role[]) => Boolean(user && roles.includes(user.role)),
    }),
    [user, login, logout],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}
