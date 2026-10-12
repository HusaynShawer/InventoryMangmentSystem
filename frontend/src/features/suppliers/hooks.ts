import { suppliersApi, type SupplierInput } from '@/api/suppliers'
import { queryKeys } from '@/lib/constants'
import { createCrudHooks } from '@/lib/crud'
import type { Supplier } from '@/types'

export const supplierHooks = createCrudHooks<Supplier, SupplierInput>(
  queryKeys.suppliers,
  suppliersApi,
  'Supplier',
)
