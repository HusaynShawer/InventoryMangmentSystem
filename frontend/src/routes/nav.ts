import {
  ArrowLeftRight,
  Boxes,
  ClipboardList,
  LayoutDashboard,
  SlidersHorizontal,
  Package,
  ShoppingCart,
  Tags,
  Truck,
  Users,
  Warehouse,
} from 'lucide-react'
import { paths } from './paths'
import type { Role } from '@/types'

export interface NavItem {
  to: string
  label: string
  icon: typeof LayoutDashboard
  /** Roles allowed to see/use the page, matching the backend policies. */
  roles: Role[]
}

const all: Role[] = ['ADMIN', 'Supplier', 'User']

// Role lists intentionally mirror the controllers:
// products/stock reads are public, everything else is AdminOnly.
export const navItems: NavItem[] = [
  { to: paths.dashboard, label: 'Dashboard', icon: LayoutDashboard, roles: all },
  { to: paths.products, label: 'Products', icon: Package, roles: all },
  { to: '/product-attributes', label: 'Attributes', icon: SlidersHorizontal, roles: ['ADMIN'] },
  { to: paths.categories, label: 'Categories', icon: Tags, roles: ['ADMIN'] },
  { to: paths.suppliers, label: 'Suppliers', icon: Truck, roles: all },
  { to: paths.warehouses, label: 'Warehouses', icon: Warehouse, roles: ['ADMIN'] },
  { to: paths.stock, label: 'Stock', icon: Boxes, roles: all },
  { to: paths.stockMovements, label: 'Stock Movements', icon: ArrowLeftRight, roles: ['ADMIN'] },
  { to: paths.purchases, label: 'Purchases', icon: ShoppingCart, roles: ['ADMIN'] },
  { to: paths.sales, label: 'Sales', icon: ClipboardList, roles: ['ADMIN'] },
  { to: paths.users, label: 'Users', icon: Users, roles: ['ADMIN'] },
]
