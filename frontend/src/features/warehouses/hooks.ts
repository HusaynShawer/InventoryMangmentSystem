import { warehousesApi } from '@/api/warehouses'
import { queryKeys } from '@/lib/constants'
import { createCrudHooks } from '@/lib/crud'
import type { Warehouse, WarehouseInput } from '@/types'

export const warehouseHooks = createCrudHooks<Warehouse, WarehouseInput>(
  queryKeys.warehouses,
  warehousesApi,
  'Warehouse',
)
