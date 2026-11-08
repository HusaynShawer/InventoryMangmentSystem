import { useQuery } from '@tanstack/react-query'
import { useParams } from 'react-router-dom'
import { productsApi } from '@/api/products'
import { extractErrorMessage } from '@/api/client'
import { PageHeader } from '@/components/data/PageHeader'
import { ErrorState, TableSkeleton } from '@/components/data/States'
import { formatCurrency } from '@/lib/utils'
import { ProductVariantsManager } from './variants'

export function ProductDetailPage() {
  const { id } = useParams<{ id: string }>()

  const { data: product, isLoading, error, refetch } = useQuery({
    queryKey: ['product', id],
    queryFn: () => productsApi.get(id!),
    enabled: Boolean(id),
  })

  if (isLoading) {
    return (
      <>
        <PageHeader title="Product" />
        <TableSkeleton rows={4} cols={4} />
      </>
    )
  }

  if (error) {
    return (
      <>
        <PageHeader title="Product" />
        <div className="rounded-lg border border-border bg-card">
          <ErrorState message={extractErrorMessage(error)} onRetry={refetch} />
        </div>
      </>
    )
  }

  if (!product) {
    return (
      <>
        <PageHeader title="Product" />
        <p className="text-sm text-muted-foreground">Product not found.</p>
      </>
    )
  }

  return (
    <>
      <PageHeader title={product.name ?? 'Product'} description={`SKU: ${product.sku ?? '—'}`} />

      <div className="rounded-md border p-4">
        <div className="grid grid-cols-2 gap-2 text-sm">
          <div>Category ID: {product.categoryId}</div>
          <div>Unit price: {formatCurrency(product.unitPrice)}</div>
          <div>Reorder level: {product.reorderLevel}</div>
        </div>
      </div>

      {id && <ProductVariantsManager productId={id} />}
    </>
  )
}
