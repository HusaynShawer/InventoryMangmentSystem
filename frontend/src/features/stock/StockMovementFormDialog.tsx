import { useEffect, useMemo } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Dialog } from '@/components/ui/dialog'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Select } from '@/components/ui/select'
import { Textarea } from '@/components/ui/textarea'
import { FormField } from '@/components/ui/form-field'
import { productHooks } from '@/features/products/hooks'
import { warehouseHooks } from '@/features/warehouses/hooks'
import { toVariantOptions, variantLabel } from '@/lib/variants'
import { useCreateStockMovement } from './hooks'

const MOVEMENT_TYPES = [
  { value: 1, label: 'Purchase in' },
  { value: 2, label: 'Sale out' },
  { value: 5, label: 'Transfer' },
  { value: 6, label: 'Adjustment' },
  { value: 7, label: 'Transfer in' },
  { value: 8, label: 'Transfer out' },
] as const

const schema = z.object({
  productVariantId: z.string().uuid('Select a product variant'),
  warehouseId: z.string().uuid('Select a warehouse'),
  movementType: z.coerce.number().int().min(1, 'Select a movement type'),
  quantityDelta: z.coerce
    .number()
    .int()
    .refine((value) => value !== 0, 'Quantity delta cannot be zero'),
  note: z.string().optional(),
})
type FormValues = z.infer<typeof schema>

/** Manual stock movement entry. Purchases/sales create these automatically too. */
export function StockMovementFormDialog({ open, onClose }: { open: boolean; onClose: () => void }) {
  const create = useCreateStockMovement()
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
        productVariantId: '',
        warehouseId: '',
        movementType: 6,
        quantityDelta: 1,
        note: '',
      })
    }
  }, [open, reset])

  const onSubmit = async (values: FormValues) => {
    await create.mutateAsync({
      productVariantId: values.productVariantId,
      warehouseId: values.warehouseId,
      movementType: values.movementType,
      quantityDelta: values.quantityDelta,
      note: values.note || undefined,
    })
    onClose()
  }

  return (
    <Dialog
      open={open}
      onClose={onClose}
      title="New stock movement"
      description="Positive quantities add stock; negative quantities remove it."
      footer={
        <>
          <Button variant="outline" onClick={onClose} disabled={create.isPending}>
            Cancel
          </Button>
          <Button type="submit" form="movement-form" loading={create.isPending}>
            Create
          </Button>
        </>
      }
    >
      <form id="movement-form" onSubmit={handleSubmit(onSubmit)} className="space-y-4">
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
        <div className="grid gap-4 sm:grid-cols-2">
          <FormField label="Quantity delta" htmlFor="quantityDelta" error={errors.quantityDelta?.message} required>
            <Input id="quantityDelta" type="number" {...register('quantityDelta')} />
          </FormField>
          <FormField label="Movement type" htmlFor="movementType" error={errors.movementType?.message} required>
            <Select id="movementType" {...register('movementType')}>
              {MOVEMENT_TYPES.map((type) => (
                <option key={type.value} value={type.value}>
                  {type.label}
                </option>
              ))}
            </Select>
          </FormField>
        </div>
        <FormField label="Note" htmlFor="note" error={errors.note?.message}>
          <Textarea id="note" placeholder="Optional note" {...register('note')} />
        </FormField>
      </form>
    </Dialog>
  )
}
