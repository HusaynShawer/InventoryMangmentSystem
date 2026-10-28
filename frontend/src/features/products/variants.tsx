import { useState } from 'react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { FormField } from '@/components/ui/form-field'
import { Dialog } from '@/components/ui/dialog'
import { productVariantsApi } from '@/api/productVariants'
import { productAttributesApi } from '@/api/productAttributes'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import type { ProductVariant, CreateProductVariantInput, ProductAttribute } from '@/types'

interface Props {
  productId: string
}

export function ProductVariantsManager({ productId }: Props) {
  const queryClient = useQueryClient()
  const { data: variants = [] } = useQuery({
    queryKey: ['product-variants', productId],
    queryFn: () => productVariantsApi.listByProduct(productId),
  })
  const { data: attributes = [] } = useQuery({
    queryKey: ['product-attributes'],
    queryFn: () => productAttributesApi.list(),
  })

  const [open, setOpen] = useState(false)
  const [sku, setSku] = useState('')
  const [barcode, setBarcode] = useState('')
  const [priceOverride, setPriceOverride] = useState<string>('')
  const [selectedValues, setSelectedValues] = useState<string[]>([])

  const createMutation = useMutation({
    mutationFn: (input: CreateProductVariantInput) => productVariantsApi.create(productId, input),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['product-variants', productId] })
      setOpen(false)
      setSku('')
      setBarcode('')
      setPriceOverride('')
      setSelectedValues([])
    },
  })

  const removeMutation = useMutation({
    mutationFn: (id: string) => productVariantsApi.remove(productId, id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['product-variants', productId] })
    },
  })

  const onSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    const input: CreateProductVariantInput = {
      sku,
      barcode: barcode || undefined,
      priceOverride: priceOverride === '' ? undefined : Number(priceOverride),
      attributeValueIds: selectedValues,
    }
    createMutation.mutate(input)
  }

  const toggleValue = (id: string) => {
    setSelectedValues((prev) => (prev.includes(id) ? prev.filter((v) => v !== id) : [...prev, id]))
  }

  return (
    <div className="space-y-3">
      <div className="flex items-center justify-between">
        <h3 className="text-sm font-medium">Variants</h3>
        <Button size="sm" onClick={() => setOpen(true)}>
          Add variant
        </Button>
      </div>
      <div className="rounded-md border">
        <table className="w-full text-sm">
          <thead>
            <tr className="border-b bg-muted/40">
              <th className="p-2 text-left">SKU</th>
              <th className="p-2 text-left">Barcode</th>
              <th className="p-2 text-left">Attributes</th>
              <th className="p-2 text-left">Effective price</th>
              <th className="p-2 text-right">Actions</th>
            </tr>
          </thead>
          <tbody>
              {variants.map((v: any) => (
                <tr key={v.id} className="border-b">
                  <td className="p-2">{v.sku}</td>
                  <td className="p-2">{v.barcode || '-'}</td>
                  <td className="p-2">{v.attributes?.join(', ') || '-'}</td>
                  <td className="p-2">{v.effectivePrice}</td>
                  <td className="p-2 text-right">
                    <Button size="sm" variant="outline" onClick={() => removeMutation.mutate(v.id!)}>
                      Remove
                    </Button>
                  </td>
                </tr>
              ))}
            {variants.length === 0 && (
              <tr>
                <td className="p-2 text-muted-foreground" colSpan={5}>
                  No variants yet
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>

      <Dialog open={open} onClose={() => setOpen(false)} title="Add product variant" className="max-w-lg">
        <form onSubmit={onSubmit} className="space-y-3">
          <FormField label="SKU" required>
            <Input value={sku} onChange={(e) => setSku(e.target.value)} required />
          </FormField>
          <FormField label="Barcode">
            <Input value={barcode} onChange={(e) => setBarcode(e.target.value)} />
          </FormField>
          <FormField label="Price override">
            <Input type="number" step="0.01" value={priceOverride} onChange={(e) => setPriceOverride(e.target.value)} />
          </FormField>
          {attributes.length > 0 && (
            <div className="space-y-2">
              <div className="text-sm font-medium">Attributes</div>
              {attributes.map((attr: ProductAttribute) => (
                <div key={(attr as any).id} className="space-y-1">
                  <div className="text-xs text-muted-foreground">{(attr as any).name}</div>
                  <div className="flex flex-wrap gap-2">
                    {((attr as any).values || []).map((val: any) => (
                      <Button
                        type="button"
                        key={val.id}
                        size="sm"
                        variant={selectedValues.includes(val.id!) ? ('default' as any) : ('outline' as any)}
                        onClick={() => toggleValue(val.id!)}
                      >
                        {val.value}
                      </Button>
                    ))}
                  </div>
                </div>
              ))}
            </div>
          )}
          <div className="flex justify-end gap-2">
            <Button type="button" variant="outline" onClick={() => setOpen(false)}>
              Cancel
            </Button>
            <Button type="submit" loading={createMutation.isPending}>
              Create
            </Button>
          </div>
        </form>
      </Dialog>
    </div>
  )
}
