import { api, getList } from './client'
import type { Product, Warehouse, WarehouseInput } from '@/types'

export const warehousesApi = {
  list: () => getList<Warehouse>('/Warehouse'),
  get: (id: string) => api.get<Warehouse>(`/Warehouse/${id}`).then((r) => r.data),
  create: (input: WarehouseInput) => api.post<Warehouse>('/Warehouse', input).then((r) => r.data),
  update: (id: string, input: WarehouseInput) =>
    api.put(`/Warehouse/${id}`, input).then((r) => r.data),
  remove: (id: string) => api.delete(`/Warehouse/${id}`).then((r) => r.data),
  // Admin-only helpers exposing products tied to the signed-in user's warehouses.
  products: () => getList<Product>('/Warehouse/products'),
  lowProducts: () => getList<Product>('/Warehouse/products/low'),
}
