import { ChevronLeft, ChevronRight } from 'lucide-react'
import { Button } from '@/components/ui/button'

interface PaginationProps {
  page: number
  pageCount: number
  total: number
  onPageChange: (page: number) => void
}

export function Pagination({ page, pageCount, total, onPageChange }: PaginationProps) {
  if (total === 0) return null
  return (
    <div className="flex items-center justify-between gap-4 border-t border-border px-4 py-3 text-sm">
      <span className="text-muted-foreground">
        Page {page} of {Math.max(pageCount, 1)} · {total} record{total === 1 ? '' : 's'}
      </span>
      <div className="flex gap-2">
        <Button
          variant="outline"
          size="sm"
          onClick={() => onPageChange(page - 1)}
          disabled={page <= 1}
        >
          <ChevronLeft className="h-4 w-4" />
          Prev
        </Button>
        <Button
          variant="outline"
          size="sm"
          onClick={() => onPageChange(page + 1)}
          disabled={page >= pageCount}
        >
          Next
          <ChevronRight className="h-4 w-4" />
        </Button>
      </div>
    </div>
  )
}
