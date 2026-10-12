import { type ReactNode } from 'react'
import { ShieldAlert } from 'lucide-react'
import { useAuth } from './useAuth'
import type { Role } from '@/types'

/**
 * Mirrors the backend authorization policies. Hiding UI is a convenience only —
 * the API still enforces access, so this never replaces real security.
 */
export function RoleGuard({ allow, children }: { allow: Role[]; children: ReactNode }) {
  const { hasRole } = useAuth()

  if (!hasRole(...allow)) {
    return (
      <div className="flex flex-col items-center justify-center gap-2 rounded-lg border border-border bg-card py-20 text-center">
        <ShieldAlert className="h-8 w-8 text-destructive" />
        <p className="font-medium">Access restricted</p>
        <p className="max-w-sm text-sm text-muted-foreground">
          Your role does not have permission to view this page.
        </p>
      </div>
    )
  }
  return <>{children}</>
}
