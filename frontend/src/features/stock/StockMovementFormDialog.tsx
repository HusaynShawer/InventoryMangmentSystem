import { useEffect } from 'react'
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
import { useAuth } from '@/features/auth/useAuth'
import { useCreateStockMovement } from './hooks'

const schema = z.object({
  productId: z.string().uuid('Select a product'),
  warehouseId: z.string().uuid('Select a warehouse'),
  quantity: z.coerce.number().int(),
  movementType: z.string().min(1, 'Movement type is required'),
  note: z.string().optional(),
})
type FormValues = z.infer<typeof schema>

/** Manual stock movement entry. Purchases/sales create these automatically too. */
export function StockMovementFormDialog({ open, onClose }: { open: boolean; onClose: () => void }) {
  const create = useCreateStockMovement()
  const { user } = useAuth()
  const { data: products = [] } = productHooks.useList()
  const { data: warehouses = [] } = warehouseHooks.useList()

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<FormValues>({ resolver: zodResolver(schema) })

  useEffect(() => {
    if (open) {
      reset({
        productId: '',
        warehouseId: '',
        quantity: 0,
        movementType: 'Adjustment',
        note: '',
      })
    }
  }, [open, reset])

  const onSubmit = async (values: FormValues) => {
    await create.mutateAsync({
      productId: values.productId,
      warehouseId: values.warehouseId,
      userId: user?.userId ?? undefined,
      quantity: values.quantity,
      movementType: values.movementType,
      referenceId: undefined,
      date: new Date().toISOString(),
      note: values.note,
    })
    onClose()
  }

  return (
    <Dialog
      open={open}
      onClose={onClose}
      title="New stock movement"
      description="ReferenceId is optional here since the backend generates it for purchases/sales."
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
        <FormField label="Product" htmlFor="productId" error={errors.productId?.message} required>
          <Select id="productId" {...register('productId')}>
            <option value="">Select a product…</option>
            {products.map((product) => (
              <option key={product.id} value={product.id}>
                {product.name} ({product.sku})
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
          <FormField label="Quantity" htmlFor="quantity" error={errors.quantity?.message} required>
            <Input id="quantity" type="number" {...register('quantity')} />
          </FormField>
          <FormField label="Movement type" htmlFor="movementType" error={errors.movementType?.message} required>
            <Input id="movementType" placeholder="Adjustment / Damage / Count" {...register('movementType')} />
          </FormField>
        </div>
        <FormField label="Note" htmlFor="note" error={errors.note?.message}>
          <Textarea id="note" placeholder="Optional note" {...register('note')} />
        </FormField>
      </form>
    </Dialog>
  )
}
