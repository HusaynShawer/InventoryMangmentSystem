import { productsApi } from '@/api/products'
import { queryKeys } from '@/lib/constants'
import { createCrudHooks } from '@/lib/crud'
import type { Product, ProductInput } from '@/types'

export const productHooks = createCrudHooks<Product, ProductInput>(
  queryKeys.products,
  productsApi,
  'Product',
)
