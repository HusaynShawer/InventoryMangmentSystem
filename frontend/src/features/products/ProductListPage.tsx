import { useMemo, useState } from 'react'
import { Pencil, Plus, Trash2 } from 'lucide-react'
import { useQuery } from '@tanstack/react-query'
import { PageHeader } from '@/components/data/PageHeader'
import { DataTable, type Column } from '@/components/data/DataTable'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { ConfirmDialog } from '@/components/ui/confirm-dialog'
import { categoriesApi } from '@/api/categories'
import { extractErrorMessage } from '@/api/client'
import { useAuth } from '@/features/auth/useAuth'
import { queryKeys } from '@/lib/constants'
import { formatCurrency, formatNumber, shortId } from '@/lib/utils'
import { ProductFormDialog } from './ProductFormDialog'
import { productHooks } from './hooks'
import type { Product } from '@/types'

export function ProductListPage() {
  const { hasRole } = useAuth()
  const isAdmin = hasRole('ADMIN')
  const { data = [], isLoading, error, refetch } = productHooks.useList()
  const deleteMutation = productHooks.useDelete()

  // Only admins may read categories; skip the request entirely for other roles.
  const { data: categories = [] } = useQuery({
    queryKey: queryKeys.categories,
    queryFn: categoriesApi.list,
    enabled: isAdmin,
  })
  const categoryNames = useMemo(
    () => new Map(categories.map((c) => [c.id, c.name])),
    [categories],
  )

  const [formOpen, setFormOpen] = useState(false)
  const [editing, setEditing] = useState<Product | null>(null)
  const [pendingDelete, setPendingDelete] = useState<Product | null>(null)

  const totalStock = (product: Product) =>
    (product.stocks ?? []).reduce((sum, stock) => sum + (stock.quantity ?? 0), 0)

  const columns: Column<Product>[] = [
    { key: 'sku', header: 'SKU', cell: (r) => <span className="font-mono text-xs">{r.sku}</span>, sortValue: (r) => r.sku ?? '' },
    { key: 'name', header: 'Name', cell: (r) => <span className="font-medium">{r.name}</span>, sortValue: (r) => r.name ?? '' },
    {
      key: 'category',
      header: 'Category',
      cell: (r) => categoryNames.get(r.categoryId ?? '') ?? shortId(r.categoryId),
      sortValue: (r) => categoryNames.get(r.categoryId ?? '') ?? '',
    },
    { key: 'price', header: 'Unit price', cell: (r) => formatCurrency(r.unitPrice), sortValue: (r) => r.unitPrice ?? 0 },
    { key: 'reorder', header: 'Reorder', cell: (r) => formatNumber(r.reorderLevel), sortValue: (r) => r.reorderLevel ?? 0 },
    {
      key: 'stock',
      header: 'On hand',
      cell: (r) => {
        const quantity = totalStock(r)
        const low = quantity <= (r.reorderLevel ?? 0)
        return <Badge variant={low ? 'warning' : 'success'}>{formatNumber(quantity)}</Badge>
      },
      sortValue: (r) => totalStock(r),
    },
  ]

  if (isAdmin) {
    columns.push({
      key: 'actions',
      header: '',
      className: 'w-24 text-right',
      cell: (row) => (
        <div className="flex justify-end gap-1">
          <Button variant="ghost" size="icon" onClick={() => { setEditing(row); setFormOpen(true) }} aria-label="Edit">
            <Pencil className="h-4 w-4" />
          </Button>
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
        title="Products"
        description="Everything you buy and sell."
        actions={
          isAdmin && (
            <Button onClick={() => { setEditing(null); setFormOpen(true) }}>
              <Plus className="h-4 w-4" />
              New product
            </Button>
          )
        }
      />

      <DataTable
        columns={columns}
        data={data}
        isLoading={isLoading}
        error={error ? extractErrorMessage(error) : null}
        onRetry={refetch}
        getRowId={(r) => r.id ?? r.sku ?? ''}
        searchPlaceholder="Search products…"
        searchFn={(r, q) => `${r.name} ${r.sku}`.toLowerCase().includes(q)}
        emptyTitle="No products"
        emptyDescription={isAdmin ? 'Add your first product to get started.' : undefined}
      />

      {isAdmin && (
        <ProductFormDialog open={formOpen} onClose={() => setFormOpen(false)} product={editing} />
      )}

      <ConfirmDialog
        open={Boolean(pendingDelete)}
        onClose={() => setPendingDelete(null)}
        onConfirm={async () => {
          if (pendingDelete?.id) await deleteMutation.mutateAsync(pendingDelete.id)
          setPendingDelete(null)
        }}
        title="Delete product"
        description={`Delete “${pendingDelete?.name}”? This cannot be undone.`}
        confirmLabel="Delete"
        loading={deleteMutation.isPending}
      />
    </>
  )
}
