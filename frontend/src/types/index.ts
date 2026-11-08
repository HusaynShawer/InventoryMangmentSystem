import type { components } from './api'

// Friendly aliases over the generated OpenAPI schema types so feature code
// never has to spell out `components['schemas'][...]`.
type Schemas = components['schemas']

export type LoginDto = Schemas['LoginDto']
export type RegisterDto = Schemas['RegisterDto']

export type Category = Schemas['CategoryResponseDTO']
export type CategoryInput = Schemas['CategoryDTO']

export type ProductVariant = {
  id: string
  productId: string
  sku: string
  barcode?: string | null
  priceOverride?: number | null
  costOverride?: number | null
  effectivePrice: number
  isActive?: boolean
  isDefault?: boolean
  attributes?: string[]
}

export type CreateProductVariantInput = {
  sku: string
  warehouseId: string
  barcode?: string
  priceOverride?: number
  costOverride?: number
  attributeValueIds?: string[]
}

export type ProductAttributeValue = {
  id: string
  productAttributeId: string
  value: string
  code: string
}

export type ProductAttribute = {
  id: string
  name: string
  code: string
  values: ProductAttributeValue[]
}

export type ProductAttributeInput = { name: string; code: string }
export type ProductAttributeValueInput = { value: string; code: string }

// The generated schema predates variants on the product response.
export type Product = Schemas['ProductResponseDTO'] & {
  variants?: ProductVariant[]
  stocks?: Stock[]
}
export type ProductInput = Schemas['ProductDTO']

export type ProductVariantOption = ProductVariant & {
  productName: string
  productSku?: string | null
}

export type Supplier = Schemas['Supplier']

export type Warehouse = Schemas['WarehouseResponseDTO']
export type WarehouseInput = Schemas['WarehouseDTO']

// Stock levels are now variant based and expose reserved/available quantities.
export type Stock = {
  id?: string
  productId?: string
  productVariantId?: string
  warehouseId?: string
  quantity?: number
  quantityOnHand?: number
  reservedQuantity?: number
  availableQuantity?: number
}

export type StockInput = {
  productId?: string
  productVariantId?: string
  warehouseId: string
  quantity: number
}

export type StockAdjustment = {
  productId?: string
  productVariantId?: string
  warehouseId: string
  reason: string
  actualQuantity: number
}

export type StockTransfer = {
  productId: string
  productVariantId: string
  fromWarehouseId: string
  toWarehouseId: string
  quantity: number
}

export type LowStock = Schemas['LowStockDto']

export type StockMovement = {
  id?: string
  stockLevelId?: string
  productId?: string
  productVariantId?: string
  warehouseId?: string
  userId?: string
  movementType?: string | number | null
  quantity?: number
  quantityDelta?: number
  resultingOnHand?: number
  referenceType?: string | null
  referenceId?: string
  note?: string | null
  date?: string
  createdAt?: string
}

export type StockMovementInput = {
  productVariantId: string
  warehouseId: string
  movementType: number
  quantityDelta: number
  note?: string
}

export type CartItem = {
  id: string
  cartId: string
  productVariantId: string
  warehouseId: string
  quantity: number
  unitPriceSnapshot: number
}

export type Cart = {
  id: string
  userId: string
  items: CartItem[]
  createdAt: string
  updatedAt: string
}

export type AddToCartInput = {
  variantId: string
  quantity: number
  warehouseId: string
}

export type Purchase = Schemas['Purchase']
export type PurchaseItem = Schemas['PurchaseItems'] & { productVariantId?: string }

export type PurchaseItemInput = {
  productId: string
  productVariantId: string
  quantity: number
  unitPrice: number
}

export type PurchaseInput = {
  supplierId: string
  warehouseId: string
  createdByUserId?: string
  date?: string
  items: PurchaseItemInput[]
}

export type Sale = Schemas['Sale']
export type SaleItem = Schemas['SaleItem'] & { productVariantId?: string }

export type SaleItemInput = {
  productId: string
  productVariantId: string
  quantity: number
  unitPrice: number
}

export type SaleInput = {
  warehouseId: string
  createdByUserID?: string
  date?: string
  totalAmount?: number
  items: SaleItemInput[]
}

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
