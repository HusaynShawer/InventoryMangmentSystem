import { api, getList } from './client'
import type { Supplier } from '@/types'

// The Supplier controller binds the full entity (no dedicated DTO), so the
// create/update payload mirrors the model.
export type SupplierInput = Pick<Supplier, 'name' | 'contactInfo'>

export const suppliersApi = {
  list: () => getList<Supplier>('/Supplier'),
  get: (id: string) => api.get<Supplier>(`/Supplier/${id}`).then((r) => r.data),
  create: (input: SupplierInput) => api.post<Supplier>('/Supplier', input).then((r) => r.data),
  update: (id: string, input: SupplierInput) =>
    api.put(`/Supplier/${id}`, { ...input, id }).then((r) => r.data),
  remove: (id: string) => api.delete(`/Supplier/${id}`).then((r) => r.data),
}
