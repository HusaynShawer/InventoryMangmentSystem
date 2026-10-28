import { useParams } from 'react-router-dom'
import { productHooks } from './hooks'
import { ProductVariantsManager } from './variants'

export function ProductDetailPage() {
  const { id } = useParams<{ id: string }>()
  const { data: products = [], isLoading } = productHooks.useList()
  const product = (products as any[]).find((p: any) => p.id === id) || (products as any)[0]

  if (isLoading) return <div className="p-4">Loading...</div>
  if (!product) return <div className="p-4">Product not found</div>

  return (
    <div className="space-y-4 p-4">
      <div>
        <h1 className="text-xl font-semibold">{product.name}</h1>
        <p className="text-sm text-muted-foreground">SKU: {product.sku}</p>
      </div>
      <div className="rounded-md border p-4">
        <div className="grid grid-cols-2 gap-2 text-sm">
          <div>Category ID: {product.categoryId}</div>
          <div>Unit price: {product.unitPrice}</div>
          <div>Reorder level: {product.reorderLevel}</div>
        </div>
      </div>
      {id && <ProductVariantsManager productId={id} />}
    </div>
  )
}
