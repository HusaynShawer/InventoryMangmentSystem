import { useEffect, useMemo, useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Select } from '@/components/ui/select'
import { FormField } from '@/components/ui/form-field'
import { Dialog } from '@/components/ui/dialog'
import { productVariantsApi } from '@/api/productVariants'
import { productAttributesApi } from '@/api/productAttributes'
import { warehousesApi } from '@/api/warehouses'
import { userWarehousesApi } from '@/api/userWarehouses'
import { useAuth } from '@/features/auth/useAuth'
import { queryKeys } from '@/lib/constants'
import { formatCurrency } from '@/lib/utils'
import type { CreateProductVariantInput, ProductAttribute } from '@/types'

interface Props {
  productId: string
}

export function ProductVariantsManager({ productId }: Props) {
  const queryClient = useQueryClient()
  const { hasRole, user } = useAuth()
  const isAdmin = hasRole('ADMIN')

  const { data: variants = [] } = useQuery({
    queryKey: ['product-variants', productId],
    queryFn: () => productVariantsApi.listByProduct(productId),
  })
  const { data: attributes = [] } = useQuery({
    queryKey: queryKeys.productAttributes,
    queryFn: () => productAttributesApi.list(),
  })
  const { data: allWarehouses = [] } = useQuery({
    queryKey: queryKeys.warehouses,
    queryFn: warehousesApi.list,
    enabled: isAdmin,
  })
  const { data: assignments = [] } = useQuery({
    queryKey: queryKeys.userWarehouses,
    queryFn: userWarehousesApi.list,
    enabled: isAdmin,
  })

  // Only offer warehouses assigned to the signed-in user; the API rejects others.
  const warehouses = useMemo(() => {
    if (!user?.userId) return []
    const allowed = new Set(
      assignments
        .filter((assignment) => assignment.userId === user.userId)
        .map((assignment) => assignment.warehouseId),
    )
    return allWarehouses.filter((warehouse) => allowed.has(warehouse.id))
  }, [allWarehouses, assignments, user?.userId])

  const [open, setOpen] = useState(false)
  const [sku, setSku] = useState('')
  const [barcode, setBarcode] = useState('')
  const [priceOverride, setPriceOverride] = useState<string>('')
  const [costOverride, setCostOverride] = useState<string>('')
  const [selectedValues, setSelectedValues] = useState<string[]>([])
  const [warehouseId, setWarehouseId] = useState('')

  useEffect(() => {
    if (!warehouseId && warehouses[0]?.id) setWarehouseId(warehouses[0].id)
  }, [warehouses, warehouseId])

  const invalidate = () => {
    queryClient.invalidateQueries({ queryKey: ['product-variants', productId] })
    queryClient.invalidateQueries({ queryKey: queryKeys.products })
    queryClient.invalidateQueries({ queryKey: queryKeys.stock })
  }

  const createMutation = useMutation({
    mutationFn: (input: CreateProductVariantInput) => productVariantsApi.create(productId, input),
    onSuccess: () => {
      invalidate()
      setOpen(false)
      setSku('')
      setBarcode('')
      setPriceOverride('')
      setCostOverride('')
      setSelectedValues([])
    },
  })

  const closeDialog = () => setOpen(false)

  const onSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (!warehouseId) return

    const input: CreateProductVariantInput = {
      sku,
      warehouseId,
      barcode: barcode || undefined,
      priceOverride: priceOverride === '' ? undefined : Number(priceOverride),
      costOverride: costOverride === '' ? undefined : Number(costOverride),
      attributeValueIds: selectedValues,
    }
    createMutation.mutate(input)
  }

  const toggleValue = (id: string) => {
    setSelectedValues((prev) => (prev.includes(id) ? prev.filter((v) => v !== id) : [...prev, id]))
  }

  return (
    <div className="space-y-3">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <h3 className="text-sm font-medium">Variants</h3>
        {isAdmin && (
          <div className="flex items-end gap-2">
            <FormField
              label="Stock warehouse"
              htmlFor="variantWarehouseId"
              hint="Used when creating or deleting variant stock."
            >
              <Select
                id="variantWarehouseId"
                value={warehouseId}
                onChange={(event) => setWarehouseId(event.target.value)}
              >
                <option value="">Select a warehouse…</option>
                {warehouses.map((warehouse) => (
                  <option key={warehouse.id} value={warehouse.id}>
                    {warehouse.name}
                  </option>
                ))}
              </Select>
            </FormField>
            <Button size="sm" onClick={() => setOpen(true)} disabled={!warehouseId}>
              Add variant
            </Button>
          </div>
        )}
      </div>
      {isAdmin && warehouses.length === 0 && (
        <p className="text-sm text-muted-foreground">
          No warehouse is assigned to your account, so variants cannot be added. Ask an
          administrator to assign a warehouse on the Users page.
        </p>
      )}
      <div className="rounded-md border">
        <table className="w-full text-sm">
          <thead>
            <tr className="border-b bg-muted/40">
              <th className="p-2 text-left">SKU</th>
              <th className="p-2 text-left">Barcode</th>
              <th className="p-2 text-left">Attributes</th>
              <th className="p-2 text-left">Effective price</th>
            </tr>
          </thead>
          <tbody>
            {variants.map((variant) => (
              <tr key={variant.id} className="border-b">
                <td className="p-2 font-mono text-xs">{variant.sku}</td>
                <td className="p-2">{variant.barcode || '-'}</td>
                <td className="p-2">{variant.attributes?.join(', ') || '-'}</td>
                <td className="p-2">{formatCurrency(variant.effectivePrice)}</td>
              </tr>
            ))}
            {variants.length === 0 && (
              <tr>
                <td className="p-2 text-muted-foreground" colSpan={4}>
                  No variants yet
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>

      <Dialog open={open} onClose={closeDialog} title="Add product variant" className="max-w-lg">
        <form onSubmit={onSubmit} className="space-y-3">
          <FormField label="SKU" required>
            <Input value={sku} onChange={(e) => setSku(e.target.value)} required />
          </FormField>
          <FormField label="Barcode">
            <Input value={barcode} onChange={(e) => setBarcode(e.target.value)} />
          </FormField>
          <div className="grid gap-3 sm:grid-cols-2">
            <FormField label="Price override">
              <Input
                type="number"
                step="0.01"
                value={priceOverride}
                onChange={(e) => setPriceOverride(e.target.value)}
              />
            </FormField>
            <FormField label="Cost override">
              <Input
                type="number"
                step="0.01"
                value={costOverride}
                onChange={(e) => setCostOverride(e.target.value)}
              />
            </FormField>
          </div>
          {attributes.length > 0 && (
            <div className="space-y-2">
              <div className="text-sm font-medium">Attributes</div>
              {attributes.map((attr: ProductAttribute) => (
                <div key={attr.id} className="space-y-1">
                  <div className="text-xs text-muted-foreground">{attr.name}</div>
                  <div className="flex flex-wrap gap-2">
                    {(attr.values || []).map((value) => (
                      <Button
                        type="button"
                        key={value.id}
                        size="sm"
                        variant={selectedValues.includes(value.id) ? 'primary' : 'outline'}
                        onClick={() => toggleValue(value.id)}
                      >
                        {value.value}
                      </Button>
                    ))}
                  </div>
                </div>
              ))}
            </div>
          )}
          <div className="flex justify-end gap-2">
            <Button type="button" variant="outline" onClick={closeDialog}>
              Cancel
            </Button>
            <Button type="submit" loading={createMutation.isPending} disabled={!warehouseId}>
              Create
            </Button>
          </div>
        </form>
      </Dialog>
    </div>
  )
}
