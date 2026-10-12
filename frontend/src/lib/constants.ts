import type { Role } from '@/types'

export const TOKEN_STORAGE_KEY = 'ims.token'
export const USER_STORAGE_KEY = 'ims.user'
export const THEME_STORAGE_KEY = 'ims.theme'

export const ROLES: Role[] = ['ADMIN', 'Supplier', 'User']

export const ROLE_LABELS: Record<Role, string> = {
  ADMIN: 'Administrator',
  Supplier: 'Supplier',
  User: 'User',
}

// Query keys keep cache invalidation predictable across features.
export const queryKeys = {
  products: ['products'] as const,
  categories: ['categories'] as const,
  suppliers: ['suppliers'] as const,
  warehouses: ['warehouses'] as const,
  stock: ['stock'] as const,
  stockMovements: ['stock-movements'] as const,
  purchases: ['purchases'] as const,
  purchaseItems: ['purchase-items'] as const,
  sales: ['sales'] as const,
  saleItems: ['sale-items'] as const,
  userWarehouses: ['user-warehouses'] as const,
}
