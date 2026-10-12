import { useEffect, useMemo, useState, type ReactNode } from 'react'
import { ArrowDown, ArrowUp, ArrowUpDown } from 'lucide-react'
import { Table, TBody, TD, TH, THead, TR } from '@/components/ui/table'
import { SearchInput } from './SearchInput'
import { EmptyState, ErrorState, TableSkeleton } from './States'
import { Pagination } from './Pagination'
import { cn } from '@/lib/utils'

export interface Column<T> {
  key: string
  header: string
  cell: (row: T) => ReactNode
  /** Providing a sort value makes the header clickable. */
  sortValue?: (row: T) => string | number
  className?: string
}

interface DataTableProps<T> {
  columns: Column<T>[]
  data: T[]
  isLoading?: boolean
  error?: string | null
  onRetry?: () => void
  searchable?: boolean
  searchPlaceholder?: string
  searchFn?: (row: T, query: string) => boolean
  pageSize?: number
  getRowId: (row: T) => string
  emptyTitle?: string
  emptyDescription?: string
  toolbar?: ReactNode
}

/**
 * Client-side table: search + sort + pagination on top of the full list the API
 * returns. The API has no server paging, so filtering here keeps pages simple.
 */
export function DataTable<T>({
  columns,
  data,
  isLoading,
  error,
  onRetry,
  searchable = true,
  searchPlaceholder = 'Search…',
  searchFn,
  pageSize = 10,
  getRowId,
  emptyTitle = 'Nothing here yet',
  emptyDescription,
  toolbar,
}: DataTableProps<T>) {
  const [query, setQuery] = useState('')
  const [sortKey, setSortKey] = useState<string | null>(null)
  const [sortDir, setSortDir] = useState<'asc' | 'desc'>('asc')
  const [page, setPage] = useState(1)

  const filtered = useMemo(() => {
    if (!query.trim()) return data
    const needle = query.trim().toLowerCase()
    const matcher =
      searchFn ??
      ((row: T) =>
        JSON.stringify(row).toLowerCase().includes(needle))
    return data.filter((row) => matcher(row, needle))
  }, [data, query, searchFn])

  const sorted = useMemo(() => {
    if (!sortKey) return filtered
    const column = columns.find((c) => c.key === sortKey)
    if (!column?.sortValue) return filtered
    const copy = [...filtered]
    copy.sort((a, b) => {
      const av = column.sortValue!(a)
      const bv = column.sortValue!(b)
      if (av < bv) return sortDir === 'asc' ? -1 : 1
      if (av > bv) return sortDir === 'asc' ? 1 : -1
      return 0
    })
    return copy
  }, [filtered, sortKey, sortDir, columns])

  const pageCount = Math.max(Math.ceil(sorted.length / pageSize), 1)

  useEffect(() => {
    setPage(1)
  }, [query, data.length])

  useEffect(() => {
    if (page > pageCount) setPage(pageCount)
  }, [page, pageCount])

  const pageRows = sorted.slice((page - 1) * pageSize, page * pageSize)

  const toggleSort = (column: Column<T>) => {
    if (!column.sortValue) return
    if (sortKey === column.key) {
      setSortDir((dir) => (dir === 'asc' ? 'desc' : 'asc'))
    } else {
      setSortKey(column.key)
      setSortDir('asc')
    }
  }

  return (
    <div className="overflow-hidden rounded-lg border border-border bg-card">
      {(searchable || toolbar) && (
        <div className="flex flex-col gap-3 border-b border-border p-4 sm:flex-row sm:items-center sm:justify-between">
          {searchable ? (
            <SearchInput
              value={query}
              onChange={setQuery}
              placeholder={searchPlaceholder}
              className="w-full sm:max-w-xs"
            />
          ) : (
            <div />
          )}
          {toolbar && <div className="flex items-center gap-2">{toolbar}</div>}
        </div>
      )}

      {isLoading ? (
        <TableSkeleton cols={columns.length} />
      ) : error ? (
        <ErrorState message={error} onRetry={onRetry} />
      ) : pageRows.length === 0 ? (
        <EmptyState title={query ? 'No matches' : emptyTitle} description={query ? `Nothing matches “${query}”.` : emptyDescription} />
      ) : (
        <Table>
          <THead>
            <TR className="hover:bg-transparent">
              {columns.map((column) => (
                <TH
                  key={column.key}
                  className={cn(column.sortValue && 'cursor-pointer select-none', column.className)}
                  onClick={() => toggleSort(column)}
                >
                  <span className="inline-flex items-center gap-1">
                    {column.header}
                    {column.sortValue &&
                      (sortKey === column.key ? (
                        sortDir === 'asc' ? (
                          <ArrowUp className="h-3.5 w-3.5" />
                        ) : (
                          <ArrowDown className="h-3.5 w-3.5" />
                        )
                      ) : (
                        <ArrowUpDown className="h-3.5 w-3.5 opacity-40" />
                      ))}
                  </span>
                </TH>
              ))}
            </TR>
          </THead>
          <TBody>
            {pageRows.map((row) => (
              <TR key={getRowId(row)}>
                {columns.map((column) => (
                  <TD key={column.key} className={column.className}>
                    {column.cell(row)}
                  </TD>
                ))}
              </TR>
            ))}
          </TBody>
        </Table>
      )}

      <Pagination page={page} pageCount={pageCount} total={sorted.length} onPageChange={setPage} />
    </div>
  )
}
