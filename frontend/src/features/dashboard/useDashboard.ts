import { useMemo } from 'react'
import { useQuery } from '@tanstack/react-query'
import { productsApi } from '@/api/products'
import { stockApi } from '@/api/stock'
import { salesApi } from '@/api/sales'
import { purchasesApi } from '@/api/purchases'
import { warehousesApi } from '@/api/warehouses'
import { queryKeys } from '@/lib/constants'
import { isSameDay } from '@/lib/utils'

/**
 * Builds dashboard metrics purely from data the existing endpoints expose.
 * Sales/purchases are fetched in full and aggregated on the client since the
 * API has no summary/report endpoint.
 */
export function useDashboard(canSeeFinancials: boolean, canSeeWarehouses: boolean) {
  const productsQuery = useQuery({
    queryKey: queryKeys.products,
    queryFn: productsApi.list,
  })
  const stockQuery = useQuery({
    queryKey: queryKeys.stock,
    queryFn: stockApi.list,
  })
  const salesQuery = useQuery({
    queryKey: queryKeys.sales,
    queryFn: salesApi.list,
    enabled: canSeeFinancials,
  })
  const purchasesQuery = useQuery({
    queryKey: queryKeys.purchases,
    queryFn: purchasesApi.list,
    enabled: canSeeFinancials,
  })
  const warehousesQuery = useQuery({
    queryKey: queryKeys.warehouses,
    queryFn: warehousesApi.list,
    enabled: canSeeWarehouses,
  })

  const metrics = useMemo(() => {
    const products = productsQuery.data ?? []
    const stock = stockQuery.data ?? []
    const sales = salesQuery.data ?? []
    const purchases = purchasesQuery.data ?? []

    const productsById = new Map(products.map((p) => [p.id, p]))
    const lowStock = stock.filter((s) => {
      const product = productsById.get(s.productId ?? '')
      return product ? (s.quantity ?? 0) <= (product.reorderLevel ?? 0) : false
    })

    const todaysSales = sales.filter((s) => isSameDay(s.date))
    const todaysPurchases = purchases.filter((p) => isSameDay(p.date))

    // Seven day trend for the chart (labels are weekday names).
    const days: { label: string; sales: number; purchases: number }[] = []
    for (let offset = 6; offset >= 0; offset -= 1) {
      const day = new Date()
      day.setDate(day.getDate() - offset)
      const label = day.toLocaleDateString(undefined, { weekday: 'short' })
      const salesTotal = sales
        .filter((s) => isSameDay(s.date, day))
        .reduce((sum, s) => sum + (s.totalAmount ?? 0), 0)
      const purchaseTotal = purchases
        .filter((p) => isSameDay(p.date, day))
        .reduce((sum, p) => sum + (p.totalAmount ?? 0), 0)
      days.push({ label, sales: salesTotal, purchases: purchaseTotal })
    }

    return {
      productCount: products.length,
      lowStockCount: lowStock.length,
      todaysSalesCount: todaysSales.length,
      todaysSalesTotal: todaysSales.reduce((sum, s) => sum + (s.totalAmount ?? 0), 0),
      todaysPurchasesCount: todaysPurchases.length,
      todaysPurchasesTotal: todaysPurchases.reduce((sum, p) => sum + (p.totalAmount ?? 0), 0),
      trend: days,
      stock,
    }
  }, [productsQuery.data, stockQuery.data, salesQuery.data, purchasesQuery.data])

  const stockByWarehouse = useMemo(() => {
    const warehouseNames = new Map(
      (warehousesQuery.data ?? []).map((w) => [w.id, w.name ?? 'Warehouse']),
    )
    const totals = new Map<string, number>()
    for (const record of metrics.stock) {
      const key = record.warehouseId ?? 'unknown'
      totals.set(key, (totals.get(key) ?? 0) + (record.quantity ?? 0))
    }
    return Array.from(totals.entries()).map(([id, quantity]) => ({
      name: warehouseNames.get(id) ?? id.slice(0, 8),
      quantity,
    }))
  }, [metrics.stock, warehousesQuery.data])

  return {
    metrics,
    stockByWarehouse,
    isLoading:
      productsQuery.isLoading ||
      stockQuery.isLoading ||
      (canSeeFinancials && (salesQuery.isLoading || purchasesQuery.isLoading)),
    error: productsQuery.error ?? stockQuery.error ?? salesQuery.error ?? purchasesQuery.error,
    refetch: () => {
      productsQuery.refetch()
      stockQuery.refetch()
      if (canSeeFinancials) {
        salesQuery.refetch()
        purchasesQuery.refetch()
      }
    },
  }
}
