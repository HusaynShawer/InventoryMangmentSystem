import { useEffect, useMemo } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Dialog } from '@/components/ui/dialog'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Select } from '@/components/ui/select'
import { FormField } from '@/components/ui/form-field'
import { productHooks } from '@/features/products/hooks'
import { warehouseHooks } from '@/features/warehouses/hooks'
import { toVariantOptions, variantLabel } from '@/lib/variants'
import { useAdjustStock } from './hooks'

const schema = z.object({
  productVariantId: z.string().uuid('Select a product variant'),
  warehouseId: z.string().uuid('Select a warehouse'),
  actualQuantity: z.coerce.number().int().min(0, 'Cannot be negative'),
  reason: z.string().min(1, 'Reason is required'),
})
type FormValues = z.infer<typeof schema>

interface Props {
  open: boolean
  onClose: () => void
  defaultProductVariantId?: string
  defaultWarehouseId?: string
}

/** Set stock to a counted value; the backend records a movement for the difference. */
export function AdjustStockDialog({ open, onClose, defaultProductVariantId, defaultWarehouseId }: Props) {
  const adjust = useAdjustStock()
  const { data: products = [] } = productHooks.useList()
  const { data: warehouses = [] } = warehouseHooks.useList()
  const variants = useMemo(() => toVariantOptions(products), [products])

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<FormValues>({ resolver: zodResolver(schema) })

  useEffect(() => {
    if (open) {
      reset({
        productVariantId: defaultProductVariantId ?? '',
        warehouseId: defaultWarehouseId ?? '',
        actualQuantity: 0,
        reason: 'Stock count adjustment',
      })
    }
  }, [open, defaultProductVariantId, defaultWarehouseId, reset])

  const onSubmit = async (values: FormValues) => {
    const variant = variants.find((option) => option.id === values.productVariantId)
    if (!variant) return

    await adjust.mutateAsync({
      productId: variant.productId,
      productVariantId: variant.id,
      warehouseId: values.warehouseId,
      actualQuantity: values.actualQuantity,
      reason: values.reason,
    })
    onClose()
  }

  return (
    <Dialog
      open={open}
      onClose={onClose}
      title="Adjust stock"
      description="Sets the on-hand quantity to the value you count. Requires access to the warehouse."
      footer={
        <>
          <Button variant="outline" onClick={onClose} disabled={adjust.isPending}>
            Cancel
          </Button>
          <Button type="submit" form="adjust-stock-form" loading={adjust.isPending}>
            Save adjustment
          </Button>
        </>
      }
    >
      <form id="adjust-stock-form" onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <FormField
          label="Product variant"
          htmlFor="productVariantId"
          error={errors.productVariantId?.message}
          required
        >
          <Select id="productVariantId" {...register('productVariantId')}>
            <option value="">Select a product variant…</option>
            {variants.map((variant) => (
              <option key={variant.id} value={variant.id}>
                {variantLabel(variant)}
              </option>
            ))}
          </Select>
        </FormField>
        <FormField label="Warehouse" htmlFor="warehouseId" error={errors.warehouseId?.message} required>
          <Select id="warehouseId" {...register('warehouseId')}>
            <option value="">Select a warehouse…</option>
            {warehouses.map((warehouse) => (
              <option key={warehouse.id} value={warehouse.id}>
                {warehouse.name}
              </option>
            ))}
          </Select>
        </FormField>
        <FormField label="Actual quantity" htmlFor="actualQuantity" error={errors.actualQuantity?.message} required>
          <Input id="actualQuantity" type="number" min={0} {...register('actualQuantity')} />
        </FormField>
        <FormField label="Reason" htmlFor="reason" error={errors.reason?.message} required>
          <Input id="reason" placeholder="Cycle count / damage / correction" {...register('reason')} />
        </FormField>
      </form>
    </Dialog>
  )
}
