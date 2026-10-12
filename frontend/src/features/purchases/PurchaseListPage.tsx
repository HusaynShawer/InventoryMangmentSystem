import { useMemo, useState } from 'react'
import { Plus, Trash2 } from 'lucide-react'
import { useNavigate } from 'react-router-dom'
import { PageHeader } from '@/components/data/PageHeader'
import { DataTable, type Column } from '@/components/data/DataTable'
import { Button } from '@/components/ui/button'
import { ConfirmDialog } from '@/components/ui/confirm-dialog'
import { extractErrorMessage } from '@/api/client'
import { supplierHooks } from '@/features/suppliers/hooks'
import { warehouseHooks } from '@/features/warehouses/hooks'
import { formatCurrency, formatDateTime, shortId } from '@/lib/utils'
import { paths } from '@/routes/paths'
import { purchaseHooks } from './hooks'
import type { Purchase } from '@/types'

export function PurchaseListPage() {
  const navigate = useNavigate()
  const { data = [], isLoading, error, refetch } = purchaseHooks.useList()
  const deleteMutation = purchaseHooks.useDelete()
  const { data: suppliers = [] } = supplierHooks.useList()
  const { data: warehouses = [] } = warehouseHooks.useList()

  const supplierNames = useMemo(() => new Map(suppliers.map((s) => [s.id, s.name])), [suppliers])
  const warehouseNames = useMemo(() => new Map(warehouses.map((w) => [w.id, w.name])), [warehouses])

  const [pendingDelete, setPendingDelete] = useState<Purchase | null>(null)

  const columns: Column<Purchase>[] = [
    { key: 'date', header: 'Date', cell: (r) => formatDateTime(r.date), sortValue: (r) => r.date ?? '' },
    {
      key: 'supplier',
      header: 'Supplier',
      cell: (r) => supplierNames.get(r.supplierId ?? '') ?? shortId(r.supplierId),
    },
    {
      key: 'warehouse',
      header: 'Warehouse',
      cell: (r) => warehouseNames.get(r.warehouseId ?? '') ?? shortId(r.warehouseId),
    },
    {
      key: 'total',
      header: 'Total',
      cell: (r) => <span className="font-medium">{formatCurrency(r.totalAmount)}</span>,
      sortValue: (r) => r.totalAmount ?? 0,
    },
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
        title="Purchases"
        description="Stock received from suppliers."
        actions={
          <Button onClick={() => navigate(`${paths.purchases}/new`)}>
            <Plus className="h-4 w-4" />
            New purchase
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
        searchPlaceholder="Search purchases…"
        searchFn={(r, q) =>
          `${supplierNames.get(r.supplierId ?? '') ?? ''} ${warehouseNames.get(r.warehouseId ?? '') ?? ''}`
            .toLowerCase()
            .includes(q)
        }
        emptyTitle="No purchases"
        emptyDescription="Record a purchase to add stock."
      />

      <ConfirmDialog
        open={Boolean(pendingDelete)}
        onClose={() => setPendingDelete(null)}
        onConfirm={async () => {
          if (pendingDelete?.id) await deleteMutation.mutateAsync(pendingDelete.id)
          setPendingDelete(null)
        }}
        title="Delete purchase"
        description="Delete this purchase record? Stock already added is not reversed."
        confirmLabel="Delete"
        loading={deleteMutation.isPending}
      />
    </>
  )
}
