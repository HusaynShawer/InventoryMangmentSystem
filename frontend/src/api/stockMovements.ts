import { api, getList } from './client'
import type { StockMovement } from '@/types'

export const stockMovementsApi = {
  list: () => getList<StockMovement>('/StockMovement'),
  get: (id: string) => api.get<StockMovement>(`/StockMovement/${id}`).then((r) => r.data),
  create: (input: StockMovement) =>
    api.post<StockMovement>('/StockMovement', input).then((r) => r.data),
  update: (id: string, input: StockMovement) =>
    api.put(`/StockMovement/${id}`, { ...input, id }).then((r) => r.data),
  remove: (id: string) => api.delete(`/StockMovement/${id}`).then((r) => r.data),
}
