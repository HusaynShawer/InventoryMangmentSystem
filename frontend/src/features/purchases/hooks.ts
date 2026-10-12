import { purchasesApi, purchaseItemsApi } from '@/api/purchases'
import { queryKeys } from '@/lib/constants'
import { createCrudHooks } from '@/lib/crud'
import { useQuery } from '@tanstack/react-query'
import type { Purchase, PurchaseInput, PurchaseItem } from '@/types'

export const purchaseHooks = createCrudHooks<Purchase, PurchaseInput>(
  queryKeys.purchases,
  purchasesApi,
  'Purchase',
)

export function usePurchaseItems() {
  return useQuery<PurchaseItem[]>({
    queryKey: queryKeys.purchaseItems,
    queryFn: purchaseItemsApi.list,
  })
}
