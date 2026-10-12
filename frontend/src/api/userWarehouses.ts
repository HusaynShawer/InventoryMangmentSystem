import { api, getList } from './client'
import type { UserWarehouse } from '@/types'

export type UserWarehouseInput = Pick<UserWarehouse, 'userId' | 'warehouseId' | 'accessLevel'>

export const userWarehousesApi = {
  list: () => getList<UserWarehouse>('/UserWarehouse'),
  get: (id: string) => api.get<UserWarehouse>(`/UserWarehouse/${id}`).then((r) => r.data),
  create: (input: UserWarehouseInput) =>
    api.post<UserWarehouse>('/UserWarehouse', input).then((r) => r.data),
}
