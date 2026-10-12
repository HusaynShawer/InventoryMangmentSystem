import { useContext } from 'react'
import { AuthContext } from './AuthContext'

/** Typed accessor for the auth context. Throws if used outside AuthProvider. */
export function useAuth() {
  const context = useContext(AuthContext)
  if (!context) throw new Error('useAuth must be used within an AuthProvider')
  return context
}
