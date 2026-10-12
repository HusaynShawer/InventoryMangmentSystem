import { useMemo, useState } from 'react'
import { Plus, Trash2 } from 'lucide-react'
import { useQuery } from '@tanstack/react-query'
import { PageHeader } from '@/components/data/PageHeader'
import { DataTable, type Column } from '@/components/data/DataTable'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { ConfirmDialog } from '@/components/ui/confirm-dialog'
import { warehousesApi } from '@/api/warehouses'
import { extractErrorMessage } from '@/api/client'
import { productHooks } from '@/features/products/hooks'
import { queryKeys } from '@/lib/constants'
import { formatDateTime, formatNumber } from '@/lib/utils'
import { StockMovementFormDialog } from './StockMovementFormDialog'
import { useDeleteStockMovement, useStockMovements } from './hooks'
import type { StockMovement } from '@/types'

export function StockMovementListPage() {
  const { data = [], isLoading, error, refetch } = useStockMovements()
  const deleteMutation = useDeleteStockMovement()
  const { data: products = [] } = productHooks.useList()
  const { data: warehouses = [] } = useQuery({
    queryKey: queryKeys.warehouses,
    queryFn: warehousesApi.list,
  })

  const productNames = useMemo(() => new Map(products.map((p) => [p.id, p.name])), [products])
  const warehouseNames = useMemo(() => new Map(warehouses.map((w) => [w.id, w.name])), [warehouses])

  const [formOpen, setFormOpen] = useState(false)
  const [pendingDelete, setPendingDelete] = useState<StockMovement | null>(null)

  const columns: Column<StockMovement>[] = [
    {
      key: 'date',
      header: 'Date',
      cell: (r) => formatDateTime(r.date),
      sortValue: (r) => r.date ?? '',
    },
    {
      key: 'product',
      header: 'Product',
      cell: (r) => productNames.get(r.productId ?? '') ?? r.productId?.slice(0, 8),
    },
    {
      key: 'warehouse',
      header: 'Warehouse',
      cell: (r) => warehouseNames.get(r.warehouseId ?? '') ?? r.warehouseId?.slice(0, 8),
    },
    {
      key: 'type',
      header: 'Type',
      cell: (r) => <Badge variant="outline">{r.movementType}</Badge>,
      sortValue: (r) => r.movementType ?? '',
    },
    {
      key: 'quantity',
      header: 'Qty',
      cell: (r) => (
        <span className={(r.quantity ?? 0) < 0 ? 'text-destructive' : 'text-success'}>
          {formatNumber(r.quantity)}
        </span>
      ),
      sortValue: (r) => r.quantity ?? 0,
    },
    { key: 'note', header: 'Note', cell: (r) => <span className="text-muted-foreground">{r.note ?? '—'}</span> },
    {
      key: 'actions',
      header: '',
      className: 'w-16 text-right',
      cell: (row) => (
        <div className="flex justify-end">
          <Button variant="ghost" size="icon" onClick={() => setPendingDelete(row)} aria-label="Delete">
            <Trash2 className="h-4 w-4 text-destructive" />
          </Button>
        </div>
      ),
    },
  ]

  return (
    <>
      <PageHeader
        title="Stock Movements"
        description="Audit trail of every stock change."
        actions={
          <Button onClick={() => setFormOpen(true)}>
            <Plus className="h-4 w-4" />
            New movement
          </Button>
        }
      />

      <DataTable
        columns={columns}
        data={data}
        isLoading={isLoading}
        error={error ? extractErrorMessage(error) : null}
        onRetry={refetch}
        getRowId={(r) => r.id ?? ''}
        searchPlaceholder="Search movements…"
        searchFn={(r, q) =>
          `${r.movementType} ${r.note} ${productNames.get(r.productId ?? '') ?? ''}`
            .toLowerCase()
            .includes(q)
        }
        emptyTitle="No stock movements"
      />

      <StockMovementFormDialog open={formOpen} onClose={() => setFormOpen(false)} />

      <ConfirmDialog
        open={Boolean(pendingDelete)}
        onClose={() => setPendingDelete(null)}
        onConfirm={async () => {
          if (pendingDelete?.id) await deleteMutation.mutateAsync(pendingDelete.id)
          setPendingDelete(null)
        }}
        title="Delete movement"
        description="Remove this stock movement from the audit trail?"
        confirmLabel="Delete"
        loading={deleteMutation.isPending}
      />
    </>
  )
}
