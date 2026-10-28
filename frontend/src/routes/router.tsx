import { createBrowserRouter, Link } from 'react-router-dom'
import { AppLayout } from '@/components/layout/AppLayout'
import { ProtectedRoute } from '@/features/auth/ProtectedRoute'
import { RoleGuard } from '@/features/auth/RoleGuard'
import { LoginPage } from '@/features/auth/LoginPage'
import { DashboardPage } from '@/features/dashboard/DashboardPage'
import { ProductListPage } from '@/features/products/ProductListPage'
import { ProductDetailPage } from '@/features/products/ProductDetailPage'
import { AttributeManager } from '@/features/attributes/AttributeManager'
import { CategoryListPage } from '@/features/categories/CategoryListPage'
import { SupplierListPage } from '@/features/suppliers/SupplierListPage'
import { WarehouseListPage } from '@/features/warehouses/WarehouseListPage'
import { StockListPage } from '@/features/stock/StockListPage'
import { StockMovementListPage } from '@/features/stock/StockMovementListPage'
import { PurchaseListPage } from '@/features/purchases/PurchaseListPage'
import { PurchaseCreatePage } from '@/features/purchases/PurchaseCreatePage'
import { SaleListPage } from '@/features/sales/SaleListPage'
import { SaleCreatePage } from '@/features/sales/SaleCreatePage'
import { UserListPage } from '@/features/users/UserListPage'

function NotFound() {
  return (
    <div className="flex flex-col items-center justify-center gap-3 py-24 text-center">
      <p className="text-4xl font-bold">404</p>
      <p className="text-muted-foreground">This page does not exist.</p>
      <Link
        to="/"
        className="inline-flex h-10 items-center rounded-md bg-primary px-4 text-sm font-medium text-primary-foreground hover:bg-primary/90"
      >
        Back to dashboard
      </Link>
    </div>
  )
}

export const router = createBrowserRouter([
  { path: '/login', element: <LoginPage /> },
  {
    element: <ProtectedRoute />,
    children: [
      {
        element: <AppLayout />,
        children: [
          { index: true, element: <DashboardPage /> },
          { path: 'products', element: <ProductListPage /> },
          { path: 'products/:id', element: <ProductDetailPage /> },
          {
            path: 'product-attributes',
            element: (
              <RoleGuard allow={['ADMIN']}>
                <AttributeManager />
              </RoleGuard>
            ),
          },
          {
            path: 'categories',
            element: (
              <RoleGuard allow={['ADMIN']}>
                <CategoryListPage />
              </RoleGuard>
            ),
          },
          { path: 'suppliers', element: <SupplierListPage /> },
          {
            path: 'warehouses',
            element: (
              <RoleGuard allow={['ADMIN']}>
                <WarehouseListPage />
              </RoleGuard>
            ),
          },
          { path: 'stock', element: <StockListPage /> },
          {
            path: 'stock-movements',
            element: (
              <RoleGuard allow={['ADMIN']}>
                <StockMovementListPage />
              </RoleGuard>
            ),
          },
          {
            path: 'purchases',
            element: (
              <RoleGuard allow={['ADMIN']}>
                <PurchaseListPage />
              </RoleGuard>
            ),
          },
          {
            path: 'purchases/new',
            element: (
              <RoleGuard allow={['ADMIN']}>
                <PurchaseCreatePage />
              </RoleGuard>
            ),
          },
          {
            path: 'sales',
            element: (
              <RoleGuard allow={['ADMIN']}>
                <SaleListPage />
              </RoleGuard>
            ),
          },
          {
            path: 'sales/new',
            element: (
              <RoleGuard allow={['ADMIN']}>
                <SaleCreatePage />
              </RoleGuard>
            ),
          },
          {
            path: 'users',
            element: (
              <RoleGuard allow={['ADMIN']}>
                <UserListPage />
              </RoleGuard>
            ),
          },
          { path: '*', element: <NotFound /> },
        ],
      },
    ],
  },
])
