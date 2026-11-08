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
import { formatDateTime, formatNumber, shortId } from '@/lib/utils'
import { variantOptionMap } from '@/lib/variants'
import { StockMovementFormDialog } from './StockMovementFormDialog'
import { useDeleteStockMovement, useStockMovements } from './hooks'
import type { StockMovement } from '@/types'

const MOVEMENT_LABELS: Record<number, string> = {
  1: 'Purchase in',
  2: 'Sale out',
  3: 'Reservation',
  4: 'Release',
  5: 'Transfer',
  6: 'Adjustment',
  7: 'Transfer in',
  8: 'Transfer out',
}

function movementLabel(movementType: StockMovement['movementType']): string {
  if (movementType == null) return '—'
  const numeric = typeof movementType === 'number' ? movementType : Number(movementType)
  return MOVEMENT_LABELS[numeric] ?? String(movementType)
}

export function StockMovementListPage() {
  const { data = [], isLoading, error, refetch } = useStockMovements()
  const deleteMutation = useDeleteStockMovement()
  const { data: products = [] } = productHooks.useList()
  const { data: warehouses = [] } = useQuery({
    queryKey: queryKeys.warehouses,
    queryFn: warehousesApi.list,
  })

  const variantsById = useMemo(() => variantOptionMap(products), [products])
  const warehouseNames = useMemo(() => new Map(warehouses.map((w) => [w.id, w.name])), [warehouses])

  const [formOpen, setFormOpen] = useState(false)
  const [pendingDelete, setPendingDelete] = useState<StockMovement | null>(null)

  const columns: Column<StockMovement>[] = [
    {
      key: 'date',
      header: 'Date',
      cell: (r) => formatDateTime(r.createdAt ?? r.date),
      sortValue: (r) => r.createdAt ?? r.date ?? '',
    },
    {
      key: 'product',
      header: 'Product',
      cell: (r) => {
        const variant = variantsById.get(r.productVariantId ?? '')
        return variant?.productName ?? shortId(r.productVariantId)
      },
      sortValue: (r) => variantsById.get(r.productVariantId ?? '')?.productName ?? '',
    },
    {
      key: 'variant',
      header: 'Variant',
      cell: (r) => (
        <span className="font-mono text-xs text-muted-foreground">
          {variantsById.get(r.productVariantId ?? '')?.sku ?? shortId(r.productVariantId)}
        </span>
      ),
      sortValue: (r) => variantsById.get(r.productVariantId ?? '')?.sku ?? '',
    },
    {
      key: 'warehouse',
      header: 'Warehouse',
      cell: (r) => warehouseNames.get(r.warehouseId ?? '') ?? shortId(r.warehouseId),
    },
    {
      key: 'type',
      header: 'Type',
      cell: (r) => <Badge variant="outline">{movementLabel(r.movementType)}</Badge>,
      sortValue: (r) => movementLabel(r.movementType),
    },
    {
      key: 'quantity',
      header: 'Qty',
      cell: (r) => {
        const quantity = r.quantityDelta ?? r.quantity ?? 0
        return (
          <span className={quantity < 0 ? 'text-destructive' : 'text-success'}>
            {formatNumber(quantity)}
          </span>
        )
      },
      sortValue: (r) => r.quantityDelta ?? r.quantity ?? 0,
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
          `${movementLabel(r.movementType)} ${r.note ?? ''} ${
            variantsById.get(r.productVariantId ?? '')?.productName ?? ''
          } ${variantsById.get(r.productVariantId ?? '')?.sku ?? ''}`
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
