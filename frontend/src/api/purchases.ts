import { api, getList } from './client'
import type { Purchase, PurchaseInput, PurchaseItem } from '@/types'

export const purchasesApi = {
  list: () => getList<Purchase>('/Purchase'),
  get: (id: string) => api.get<Purchase>(`/Purchase/${id}`).then((r) => r.data),
  create: (input: PurchaseInput) => api.post<Purchase>('/Purchase', input).then((r) => r.data),
  update: (id: string, input: PurchaseInput) =>
    api.put(`/Purchase/${id}`, { ...input, id }).then((r) => r.data),
  remove: (id: string) => api.delete(`/Purchase/${id}`).then((r) => r.data),
}

// Line items live in their own (unauthenticated) controller.
export const purchaseItemsApi = {
  list: () => getList<PurchaseItem>('/PurchaseItems'),
  get: (id: string) => api.get<PurchaseItem>(`/PurchaseItems/${id}`).then((r) => r.data),
  remove: (id: string) => api.delete(`/PurchaseItems/${id}`).then((r) => r.data),
}
