import { useEffect } from 'react'
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
import { useCreateStock } from './hooks'

const schema = z.object({
  productId: z.string().uuid('Select a product'),
  warehouseId: z.string().uuid('Select a warehouse'),
  quantity: z.coerce.number().int().min(0, 'Cannot be negative'),
})
type FormValues = z.infer<typeof schema>

/** Creates a brand new stock row (product + warehouse pair). */
export function StockFormDialog({ open, onClose }: { open: boolean; onClose: () => void }) {
  const create = useCreateStock()
  const { data: products = [] } = productHooks.useList()
  const { data: warehouses = [] } = warehouseHooks.useList()

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<FormValues>({ resolver: zodResolver(schema) })

  useEffect(() => {
    if (open) reset({ productId: '', warehouseId: '', quantity: 0 })
  }, [open, reset])

  const onSubmit = async (values: FormValues) => {
    await create.mutateAsync(values)
    onClose()
  }

  return (
    <Dialog
      open={open}
      onClose={onClose}
      title="New stock record"
      description="Add stock for a product/warehouse combination that does not exist yet."
      footer={
        <>
          <Button variant="outline" onClick={onClose} disabled={create.isPending}>
            Cancel
          </Button>
          <Button type="submit" form="stock-form" loading={create.isPending}>
            Create
          </Button>
        </>
      }
    >
      <form id="stock-form" onSubmit={handleSubmit(onSubmit)} className="space-y-4">
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
        <FormField label="Quantity" htmlFor="quantity" error={errors.quantity?.message} required>
          <Input id="quantity" type="number" min={0} {...register('quantity')} />
        </FormField>
      </form>
    </Dialog>
  )
}
