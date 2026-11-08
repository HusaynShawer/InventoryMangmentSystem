import { useMemo, useState } from 'react'
import { ArrowLeftRight, Plus, ShoppingBag, SlidersHorizontal, Trash2 } from 'lucide-react'
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
import { AddToCartDialog } from '@/features/cart/AddToCartDialog'
import { queryKeys } from '@/lib/constants'
import { formatNumber, shortId } from '@/lib/utils'
import { variantOptionMap } from '@/lib/variants'
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

  const productsById = useMemo(() => new Map(products.map((p) => [p.id, p])), [products])
  const variantsById = useMemo(() => variantOptionMap(products), [products])
  const warehouseNames = useMemo(() => new Map(warehouses.map((w) => [w.id, w.name])), [warehouses])

  const [adjustOpen, setAdjustOpen] = useState(false)
  const [transferOpen, setTransferOpen] = useState(false)
  const [formOpen, setFormOpen] = useState(false)
  const [pendingDelete, setPendingDelete] = useState<Stock | null>(null)
  const [addTarget, setAddTarget] = useState<{ variantId: string; warehouseId: string } | null>(null)

  const columns: Column<Stock>[] = [
    {
      key: 'product',
      header: 'Product',
      cell: (r) => {
        const variant = variantsById.get(r.productVariantId ?? '')
        if (variant) return <span className="font-medium">{variant.productName}</span>
        return productsById.get(r.productId ?? '')?.name ?? shortId(r.productId)
      },
      sortValue: (r) =>
        variantsById.get(r.productVariantId ?? '')?.productName ??
        productsById.get(r.productId ?? '')?.name ??
        '',
    },
    {
      key: 'variant',
      header: 'Variant',
      cell: (r) => {
        const variant = variantsById.get(r.productVariantId ?? '')
        return (
          <span className="font-mono text-xs text-muted-foreground">
            {variant?.sku ?? shortId(r.productVariantId)}
          </span>
        )
      },
      sortValue: (r) => variantsById.get(r.productVariantId ?? '')?.sku ?? '',
    },
    {
      key: 'warehouse',
      header: 'Warehouse',
      cell: (r) => warehouseNames.get(r.warehouseId ?? '') ?? shortId(r.warehouseId),
    },
    {
      key: 'onHand',
      header: 'On hand',
      cell: (r) => {
        const productId =
          variantsById.get(r.productVariantId ?? '')?.productId ?? r.productId
        const product = productsById.get(productId ?? '')
        const onHand = r.quantityOnHand ?? r.quantity ?? 0
        const low = product ? onHand <= (product.reorderLevel ?? 0) : false
        return <Badge variant={low ? 'warning' : 'success'}>{formatNumber(onHand)}</Badge>
      },
      sortValue: (r) => r.quantityOnHand ?? r.quantity ?? 0,
    },
    {
      key: 'available',
      header: 'Available',
      cell: (r) => formatNumber(r.availableQuantity ?? r.quantityOnHand ?? r.quantity ?? 0),
      sortValue: (r) => r.availableQuantity ?? r.quantityOnHand ?? r.quantity ?? 0,
    },
    {
      key: 'actions',
      header: '',
      className: 'w-24 text-right',
      cell: (row) => (
        <div className="flex justify-end gap-1">
          {row.productVariantId && row.warehouseId && (
            <Button
              variant="ghost"
              size="icon"
              onClick={() =>
                setAddTarget({
                  variantId: row.productVariantId!,
                  warehouseId: row.warehouseId!,
                })
              }
              aria-label="Add to cart"
            >
              <ShoppingBag className="h-4 w-4" />
            </Button>
          )}
          {isAdmin && (
            <Button variant="ghost" size="icon" onClick={() => setPendingDelete(row)} aria-label="Delete">
              <Trash2 className="h-4 w-4 text-destructive" />
            </Button>
          )}
        </div>
      ),
    },
  ]

  return (
    <>
      <PageHeader
        title="Stock"
        description="On-hand quantities per product variant and warehouse."
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
        getRowId={(r) => r.id ?? `${r.productVariantId}-${r.warehouseId}`}
        searchPlaceholder="Search by product, variant or warehouse…"
        searchFn={(r, q) =>
          `${
            variantsById.get(r.productVariantId ?? '')?.productName ??
            productsById.get(r.productId ?? '')?.name ??
            ''
          } ${variantsById.get(r.productVariantId ?? '')?.sku ?? ''} ${
            warehouseNames.get(r.warehouseId ?? '') ?? ''
          }`
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

      <AddToCartDialog
        open={Boolean(addTarget)}
        onClose={() => setAddTarget(null)}
        defaultVariantId={addTarget?.variantId}
        defaultWarehouseId={addTarget?.warehouseId}
      />

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
