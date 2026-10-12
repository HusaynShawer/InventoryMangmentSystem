import { api, getList } from './client'
import type { LowStock, Stock, StockAdjustment, StockTransfer } from '@/types'

export const stockApi = {
  list: () => getList<Stock>('/Stock'),
  get: (id: string) => api.get<Stock>(`/Stock/${id}`).then((r) => r.data),
  create: (input: Stock) => api.post<Stock>('/Stock', input).then((r) => r.data),
  update: (id: string, input: Stock) => api.put(`/Stock/${id}`, { ...input, id }).then((r) => r.data),
  remove: (id: string) => api.delete(`/Stock/${id}`).then((r) => r.data),
  adjust: (input: StockAdjustment) => api.put('/Stock/adjust', input).then((r) => r.data),
  transfer: (input: StockTransfer) => api.post('/Stock/Transfer', input).then((r) => r.data),
  lowStock: (warehouseId: string) =>
    getList<LowStock>('/Stock/lowstock', { warehouseId }),
}
