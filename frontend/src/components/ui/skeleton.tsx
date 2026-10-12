import { type HTMLAttributes } from 'react'
import { cn } from '@/lib/utils'

/** Loading placeholder used while a table/query is fetching. */
export function Skeleton({ className, ...props }: HTMLAttributes<HTMLDivElement>) {
  return <div className={cn('animate-pulse rounded-md bg-muted', className)} {...props} />
}
