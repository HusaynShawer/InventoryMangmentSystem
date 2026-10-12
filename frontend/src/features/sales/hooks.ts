import { salesApi, saleItemsApi } from '@/api/sales'
import { queryKeys } from '@/lib/constants'
import { createCrudHooks } from '@/lib/crud'
import { useQuery } from '@tanstack/react-query'
import type { Sale, SaleInput, SaleItem } from '@/types'

export const saleHooks = createCrudHooks<Sale, SaleInput>(
  queryKeys.sales,
  salesApi,
  'Sale',
)

export function useSaleItems() {
  return useQuery<SaleItem[]>({
    queryKey: queryKeys.saleItems,
    queryFn: saleItemsApi.list,
  })
}
