import type { components } from './api'

// Friendly aliases over the generated OpenAPI schema types so feature code
// never has to spell out `components['schemas'][...]`.
type Schemas = components['schemas']

export type LoginDto = Schemas['LoginDto']
export type RegisterDto = Schemas['RegisterDto']

export type Category = Schemas['CategoryResponseDTO']
export type CategoryInput = Schemas['CategoryDTO']

export type Product = Schemas['ProductResponseDTO']
export type ProductInput = Schemas['ProductDTO']

export type ProductVariant = any
export type CreateProductVariantInput = any
export type ProductAttribute = any
export type ProductAttributeInput = any
export type ProductAttributeValueInput = any

export type Supplier = Schemas['Supplier']

export type Warehouse = Schemas['WarehouseResponseDTO']
export type WarehouseInput = Schemas['WarehouseDTO']

export type Stock = Schemas['Stock']
export type StockInput = Schemas['Stock']
export type StockAdjustment = Schemas['StockAdjustmentDTO']
export type StockTransfer = Schemas['TransferDto']
export type LowStock = Schemas['LowStockDto']

export type StockMovement = Schemas['StockMovement']
export type StockMovementInput = Schemas['StockMovement']

export type Purchase = Schemas['Purchase']
export type PurchaseInput = Schemas['PurchaseCreateDto']
export type PurchaseItem = Schemas['PurchaseItems']

export type Sale = Schemas['Sale']
export type SaleInput = Schemas['SaleDTO']
export type SaleItem = Schemas['SaleItem']

export type User = Schemas['User']
export type UserWarehouse = Schemas['UserWarehouse']

// The API groups roles exactly as the JWT does.
export type Role = 'ADMIN' | 'Supplier' | 'User'

export interface AuthUser {
  userName: string
  role: Role
  userId: string | null
  supplierId: string | null
  expiresAt: number | null
}
