import { useMemo, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { Minus, Plus, ShoppingBag, Trash2 } from 'lucide-react'
import { useQuery } from '@tanstack/react-query'
import { PageHeader } from '@/components/data/PageHeader'
import { DataTable, type Column } from '@/components/data/DataTable'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { ConfirmDialog } from '@/components/ui/confirm-dialog'
import { warehousesApi } from '@/api/warehouses'
import { extractErrorMessage } from '@/api/client'
import { productHooks } from '@/features/products/hooks'
import { useStockList } from '@/features/stock/hooks'
import { useAuth } from '@/features/auth/useAuth'
import { queryKeys } from '@/lib/constants'
import { formatCurrency, formatNumber, shortId } from '@/lib/utils'
import { paths } from '@/routes/paths'
import { AddToCartDialog } from './AddToCartDialog'
import { useCart, useClearCart, useRemoveCartItem, useUpdateCartItem } from './hooks'
import type { CartItem } from '@/types'

export function CartPage() {
  const navigate = useNavigate()
  const { hasRole } = useAuth()
  const isAdmin = hasRole('ADMIN')
  const { data: cart, isLoading, error, refetch } = useCart()
  const { data: products = [] } = productHooks.useList()
  const { data: stock = [] } = useStockList()
  const { data: warehouses = [] } = useQuery({
    queryKey: queryKeys.warehouses,
    queryFn: warehousesApi.list,
    enabled: isAdmin,
  })

  const updateItem = useUpdateCartItem()
  const removeItem = useRemoveCartItem()
  const clearCart = useClearCart()

  const [addOpen, setAddOpen] = useState(false)
  const [confirmClear, setConfirmClear] = useState(false)

  const variants = useMemo(() => {
    const map = new Map<string, { productName: string; sku: string }>()
    for (const product of products) {
      for (const variant of product.variants ?? []) {
        map.set(variant.id, {
          productName: product.name ?? '',
          sku: variant.sku,
        })
      }
    }
    return map
  }, [products])

  const warehouseNames = useMemo(
    () => new Map(warehouses.map((warehouse) => [warehouse.id, warehouse.name ?? 'Warehouse'])),
    [warehouses],
  )

  const availability = useMemo(() => {
    const map = new Map<string, number>()
    for (const row of stock) {
      if (!row.productVariantId || !row.warehouseId) continue
      map.set(
        `${row.productVariantId}:${row.warehouseId}`,
        row.availableQuantity ?? row.quantityOnHand ?? row.quantity ?? 0,
      )
    }
    return map
  }, [stock])

  const items = cart?.items ?? []
  const subtotal = items.reduce(
    (sum, item) => sum + item.quantity * (item.unitPriceSnapshot ?? 0),
    0,
  )

  const columns: Column<CartItem>[] = [
    {
      key: 'product',
      header: 'Product',
      cell: (row) => (
        <span className="font-medium">
          {variants.get(row.productVariantId)?.productName ?? shortId(row.productVariantId)}
        </span>
      ),
    },
    {
      key: 'variant',
      header: 'Variant',
      cell: (row) => (
        <span className="font-mono text-xs text-muted-foreground">
          {variants.get(row.productVariantId)?.sku ?? shortId(row.productVariantId)}
        </span>
      ),
    },
    {
      key: 'warehouse',
      header: 'Warehouse',
      cell: (row) =>
        warehouseNames.get(row.warehouseId) ?? shortId(row.warehouseId),
    },
    {
      key: 'price',
      header: 'Unit price',
      cell: (row) => formatCurrency(row.unitPriceSnapshot),
      sortValue: (row) => row.unitPriceSnapshot ?? 0,
    },
    {
      key: 'quantity',
      header: 'Quantity',
      cell: (row) => {
        const available =
          availability.get(`${row.productVariantId}:${row.warehouseId}`) ?? 0
        return (
          <div className="flex items-center gap-1">
            <Button
              variant="outline"
              size="icon"
              className="h-7 w-7"
              disabled={updateItem.isPending || row.quantity <= 1}
              onClick={() =>
                updateItem.mutate({ itemId: row.id, quantity: row.quantity - 1 })
              }
              aria-label="Decrease quantity"
            >
              <Minus className="h-3 w-3" />
            </Button>
            <span className="w-8 text-center text-sm font-medium">{row.quantity}</span>
            <Button
              variant="outline"
              size="icon"
              className="h-7 w-7"
              disabled={updateItem.isPending || available < 1}
              onClick={() =>
                updateItem.mutate({ itemId: row.id, quantity: row.quantity + 1 })
              }
              aria-label="Increase quantity"
            >
              <Plus className="h-3 w-3" />
            </Button>
          </div>
        )
      },
      sortValue: (row) => row.quantity,
    },
    {
      key: 'available',
      header: 'Available',
      cell: (row) => {
        const available =
          availability.get(`${row.productVariantId}:${row.warehouseId}`) ?? 0
        return (
          <Badge variant={available <= 0 ? 'destructive' : 'success'}>
            {formatNumber(available)}
          </Badge>
        )
      },
      sortValue: (row) =>
        availability.get(`${row.productVariantId}:${row.warehouseId}`) ?? 0,
    },
    {
      key: 'total',
      header: 'Line total',
      cell: (row) => (
        <span className="font-medium">
          {formatCurrency(row.quantity * (row.unitPriceSnapshot ?? 0))}
        </span>
      ),
      sortValue: (row) => row.quantity * (row.unitPriceSnapshot ?? 0),
    },
    {
      key: 'actions',
      header: '',
      className: 'w-16 text-right',
      cell: (row) => (
        <div className="flex justify-end">
          <Button
            variant="ghost"
            size="icon"
            disabled={removeItem.isPending}
            onClick={() => removeItem.mutate(row.id)}
            aria-label="Remove item"
          >
            <Trash2 className="h-4 w-4 text-destructive" />
          </Button>
        </div>
      ),
    },
  ]

  return (
    <>
      <PageHeader
        title="Cart"
        description="Items reserved for you across your warehouses."
        actions={
          <>
            <Button variant="outline" onClick={() => navigate(paths.stock)}>
              Browse stock
            </Button>
            <Button onClick={() => setAddOpen(true)}>
              <ShoppingBag className="h-4 w-4" />
              Add item
            </Button>
          </>
        }
      />

      <div className="mb-4 grid gap-4 sm:grid-cols-2">
        <div className="rounded-lg border border-border bg-card p-4">
          <p className="text-sm text-muted-foreground">Items</p>
          <p className="text-xl font-semibold">{formatNumber(items.length)}</p>
        </div>
        <div className="rounded-lg border border-border bg-card p-4">
          <p className="text-sm text-muted-foreground">Subtotal</p>
          <p className="text-xl font-semibold">{formatCurrency(subtotal)}</p>
        </div>
      </div>

      <DataTable
        columns={columns}
        data={items}
        isLoading={isLoading}
        error={error ? extractErrorMessage(error) : null}
        onRetry={refetch}
        getRowId={(row) => row.id}
        searchPlaceholder="Search cart…"
        searchFn={(row, query) =>
          `${variants.get(row.productVariantId)?.productName ?? ''} ${
            variants.get(row.productVariantId)?.sku ?? ''
          } ${warehouseNames.get(row.warehouseId) ?? ''}`
            .toLowerCase()
            .includes(query)
        }
        emptyTitle="Your cart is empty"
        emptyDescription="Add a product variant from the stock page to reserve it."
        toolbar={
          items.length > 0 ? (
            <Button variant="outline" onClick={() => setConfirmClear(true)} disabled={clearCart.isPending}>
              <Trash2 className="h-4 w-4" />
              Clear cart
            </Button>
          ) : undefined
        }
      />

      <AddToCartDialog open={addOpen} onClose={() => setAddOpen(false)} />

      <ConfirmDialog
        open={confirmClear}
        onClose={() => setConfirmClear(false)}
        onConfirm={async () => {
          await clearCart.mutateAsync()
          setConfirmClear(false)
        }}
        title="Clear cart"
        description="Remove every item and release its reserved stock?"
        confirmLabel="Clear cart"
        loading={clearCart.isPending}
      />
    </>
  )
}
