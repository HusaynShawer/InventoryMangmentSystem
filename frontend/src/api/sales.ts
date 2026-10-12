import { api, getList } from './client'
import type { Sale, SaleInput, SaleItem } from '@/types'

export const salesApi = {
  list: () => getList<Sale>('/Sale'),
  get: (id: string) => api.get<Sale>(`/Sale/${id}`).then((r) => r.data),
  create: (input: SaleInput) => api.post<Sale>('/Sale', input).then((r) => r.data),
  update: (id: string, input: SaleInput) =>
    api.put(`/Sale/${id}`, { ...input, id }).then((r) => r.data),
  remove: (id: string) => api.delete(`/Sale/${id}`).then((r) => r.data),
}

export const saleItemsApi = {
  list: () => getList<SaleItem>('/SaleItem'),
  get: (id: string) => api.get<SaleItem>(`/SaleItem/${id}`).then((r) => r.data),
  remove: (id: string) => api.delete(`/SaleItem/${id}`).then((r) => r.data),
}
