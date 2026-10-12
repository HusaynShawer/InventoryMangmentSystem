import { useMemo, useState } from 'react'
import { ArrowLeftRight, Plus, SlidersHorizontal, Trash2 } from 'lucide-react'
import { useQuery } from '@tanstack/react-query'
import { PageHeader } from '@/components/data/PageHeader'
import { DataTable, type Column } from '@/components/data/DataTable'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { ConfirmDialog } from '@/components/ui/confirm-dialog'
import { warehousesApi } from '@/api/warehouses'
import { extractErrorMessage } from '@/api/client'
import { productHooks } from '@/features/products/hooks'
import { useAuth } from '@/features/auth/useAuth'
import { queryKeys } from '@/lib/constants'
import { formatNumber } from '@/lib/utils'
import { AdjustStockDialog } from './AdjustStockDialog'
import { TransferStockDialog } from './TransferStockDialog'
import { StockFormDialog } from './StockFormDialog'
import { useDeleteStock, useStockList } from './hooks'
import type { Stock } from '@/types'

export function StockListPage() {
  const { hasRole } = useAuth()
  const isAdmin = hasRole('ADMIN')
  const { data = [], isLoading, error, refetch } = useStockList()
  const deleteMutation = useDeleteStock()

  const { data: products = [] } = productHooks.useList()
  // Warehouse names require an admin token, so only fetch when allowed.
  const { data: warehouses = [] } = useQuery({
    queryKey: queryKeys.warehouses,
    queryFn: warehousesApi.list,
    enabled: isAdmin,
  })

  const productNames = useMemo(() => new Map(products.map((p) => [p.id, p])), [products])
  const warehouseNames = useMemo(() => new Map(warehouses.map((w) => [w.id, w.name])), [warehouses])

  const [adjustOpen, setAdjustOpen] = useState(false)
  const [transferOpen, setTransferOpen] = useState(false)
  const [formOpen, setFormOpen] = useState(false)
  const [pendingDelete, setPendingDelete] = useState<Stock | null>(null)

  const columns: Column<Stock>[] = [
    {
      key: 'product',
      header: 'Product',
      cell: (r) => productNames.get(r.productId ?? '')?.name ?? r.productId?.slice(0, 8),
      sortValue: (r) => productNames.get(r.productId ?? '')?.name ?? '',
    },
    {
      key: 'warehouse',
      header: 'Warehouse',
      cell: (r) => warehouseNames.get(r.warehouseId ?? '') ?? r.warehouseId?.slice(0, 8),
    },
    {
      key: 'quantity',
      header: 'Quantity',
      cell: (r) => {
        const product = productNames.get(r.productId ?? '')
        const low = product ? (r.quantity ?? 0) <= (product.reorderLevel ?? 0) : false
        return <Badge variant={low ? 'warning' : 'success'}>{formatNumber(r.quantity)}</Badge>
      },
      sortValue: (r) => r.quantity ?? 0,
    },
  ]

  if (isAdmin) {
    columns.push({
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
    })
  }

  return (
    <>
      <PageHeader
        title="Stock"
        description="On-hand quantities per product and warehouse."
        actions={
          isAdmin && (
            <>
              <Button variant="outline" onClick={() => setTransferOpen(true)}>
                <ArrowLeftRight className="h-4 w-4" />
                Transfer
              </Button>
              <Button variant="outline" onClick={() => setAdjustOpen(true)}>
                <SlidersHorizontal className="h-4 w-4" />
                Adjust
              </Button>
              <Button onClick={() => setFormOpen(true)}>
                <Plus className="h-4 w-4" />
                New record
              </Button>
            </>
          )
        }
      />

      <DataTable
        columns={columns}
        data={data}
        isLoading={isLoading}
        error={error ? extractErrorMessage(error) : null}
        onRetry={refetch}
        getRowId={(r) => r.id ?? `${r.productId}-${r.warehouseId}`}
        searchPlaceholder="Search by product or warehouse…"
        searchFn={(r, q) =>
          `${productNames.get(r.productId ?? '')?.name ?? ''} ${warehouseNames.get(r.warehouseId ?? '') ?? ''}`
            .toLowerCase()
            .includes(q)
        }
        emptyTitle="No stock recorded"
      />

      {isAdmin && (
        <>
          <AdjustStockDialog open={adjustOpen} onClose={() => setAdjustOpen(false)} />
          <TransferStockDialog open={transferOpen} onClose={() => setTransferOpen(false)} />
          <StockFormDialog open={formOpen} onClose={() => setFormOpen(false)} />
        </>
      )}

      <ConfirmDialog
        open={Boolean(pendingDelete)}
        onClose={() => setPendingDelete(null)}
        onConfirm={async () => {
          if (pendingDelete?.id) await deleteMutation.mutateAsync(pendingDelete.id)
          setPendingDelete(null)
        }}
        title="Delete stock record"
        description="This removes the stock row entirely. Continue?"
        confirmLabel="Delete"
        loading={deleteMutation.isPending}
      />
    </>
  )
}
