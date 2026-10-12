import { api, getList } from './client'
import type { Product, ProductInput } from '@/types'

export const productsApi = {
  list: () => getList<Product>('/Product'),
  get: (id: string) => api.get<Product>(`/Product/${id}`).then((r) => r.data),
  create: (input: ProductInput) => api.post<Product>('/Product', input).then((r) => r.data),
  update: (id: string, input: ProductInput) =>
    api.put<Product>(`/Product/${id}`, input).then((r) => r.data),
  remove: (id: string) => api.delete(`/Product/${id}`).then((r) => r.data),
}
