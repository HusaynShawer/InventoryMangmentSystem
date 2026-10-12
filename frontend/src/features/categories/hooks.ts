import { categoriesApi } from '@/api/categories'
import { queryKeys } from '@/lib/constants'
import { createCrudHooks } from '@/lib/crud'
import type { Category, CategoryInput } from '@/types'

export const categoryHooks = createCrudHooks<Category, CategoryInput>(
  queryKeys.categories,
  categoriesApi,
  'Category',
)
