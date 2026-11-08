import type { Product, ProductVariantOption } from '@/types'

/** Flattens the product catalogue into the variants users actually transact on. */
export function toVariantOptions(products: Product[]): ProductVariantOption[] {
  return products.flatMap((product) =>
    (product.variants ?? []).map((variant) => ({
      ...variant,
      productName: product.name ?? '',
      productSku: product.sku,
    })),
  )
}

export function variantOptionMap(products: Product[]): Map<string, ProductVariantOption> {
  return new Map(toVariantOptions(products).map((variant) => [variant.id, variant]))
}

export function variantLabel(variant: ProductVariantOption): string {
  const attributes = variant.attributes?.length ? ` · ${variant.attributes.join(', ')}` : ''
  return `${variant.productName} — ${variant.sku}${attributes}`
}
