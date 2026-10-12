import { useMemo, useState } from 'react'
import { Plus, Trash2 } from 'lucide-react'
import { useNavigate } from 'react-router-dom'
import { PageHeader } from '@/components/data/PageHeader'
import { DataTable, type Column } from '@/components/data/DataTable'
import { Button } from '@/components/ui/button'
import { ConfirmDialog } from '@/components/ui/confirm-dialog'
import { extractErrorMessage } from '@/api/client'
import { warehouseHooks } from '@/features/warehouses/hooks'
import { formatCurrency, formatDateTime, shortId } from '@/lib/utils'
import { paths } from '@/routes/paths'
import { saleHooks } from './hooks'
import type { Sale } from '@/types'

export function SaleListPage() {
  const navigate = useNavigate()
  const { data = [], isLoading, error, refetch } = saleHooks.useList()
  const deleteMutation = saleHooks.useDelete()
  const { data: warehouses = [] } = warehouseHooks.useList()

  const warehouseNames = useMemo(() => new Map(warehouses.map((w) => [w.id, w.name])), [warehouses])
  const [pendingDelete, setPendingDelete] = useState<Sale | null>(null)

  const columns: Column<Sale>[] = [
    { key: 'date', header: 'Date', cell: (r) => formatDateTime(r.date), sortValue: (r) => r.date ?? '' },
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
        title="Sales"
        description="Stock sold to customers."
        actions={
          <Button onClick={() => navigate(`${paths.sales}/new`)}>
            <Plus className="h-4 w-4" />
            New sale
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
        searchPlaceholder="Search sales…"
        searchFn={(r, q) =>
          `${warehouseNames.get(r.warehouseId ?? '') ?? ''}`.toLowerCase().includes(q)
        }
        emptyTitle="No sales"
        emptyDescription="Record a sale to deduct stock."
      />

      <ConfirmDialog
        open={Boolean(pendingDelete)}
        onClose={() => setPendingDelete(null)}
        onConfirm={async () => {
          if (pendingDelete?.id) await deleteMutation.mutateAsync(pendingDelete.id)
          setPendingDelete(null)
        }}
        title="Delete sale"
        description="Delete this sale record? Stock already deducted is not restored."
        confirmLabel="Delete"
        loading={deleteMutation.isPending}
      />
    </>
  )
}
