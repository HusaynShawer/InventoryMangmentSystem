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
import { useTransferStock } from './hooks'

const schema = z
  .object({
    productId: z.string().uuid('Select a product'),
    formWarehouseId: z.string().uuid('Select the source warehouse'),
    toWarehouseId: z.string().uuid('Select the destination warehouse'),
    quantity: z.coerce.number().int().positive('Must be greater than zero'),
  })
  .refine((values) => values.formWarehouseId !== values.toWarehouseId, {
    message: 'Source and destination must differ',
    path: ['toWarehouseId'],
  })
type FormValues = z.infer<typeof schema>

export function TransferStockDialog({ open, onClose }: { open: boolean; onClose: () => void }) {
  const transfer = useTransferStock()
  const { data: products = [] } = productHooks.useList()
  const { data: warehouses = [] } = warehouseHooks.useList()

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<FormValues>({ resolver: zodResolver(schema) })

  useEffect(() => {
    if (open) reset({ productId: '', formWarehouseId: '', toWarehouseId: '', quantity: 1 })
  }, [open, reset])

  const onSubmit = async (values: FormValues) => {
    await transfer.mutateAsync(values)
    onClose()
  }

  return (
    <Dialog
      open={open}
      onClose={onClose}
      title="Transfer stock"
      description="Move quantity between two warehouses you have access to."
      footer={
        <>
          <Button variant="outline" onClick={onClose} disabled={transfer.isPending}>
            Cancel
          </Button>
          <Button type="submit" form="transfer-form" loading={transfer.isPending}>
            Transfer
          </Button>
        </>
      }
    >
      <form id="transfer-form" onSubmit={handleSubmit(onSubmit)} className="space-y-4">
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
        <div className="grid gap-4 sm:grid-cols-2">
          <FormField label="From" htmlFor="formWarehouseId" error={errors.formWarehouseId?.message} required>
            <Select id="formWarehouseId" {...register('formWarehouseId')}>
              <option value="">Select…</option>
              {warehouses.map((warehouse) => (
                <option key={warehouse.id} value={warehouse.id}>
                  {warehouse.name}
                </option>
              ))}
            </Select>
          </FormField>
          <FormField label="To" htmlFor="toWarehouseId" error={errors.toWarehouseId?.message} required>
            <Select id="toWarehouseId" {...register('toWarehouseId')}>
              <option value="">Select…</option>
              {warehouses.map((warehouse) => (
                <option key={warehouse.id} value={warehouse.id}>
                  {warehouse.name}
                </option>
              ))}
            </Select>
          </FormField>
        </div>
        <FormField label="Quantity" htmlFor="quantity" error={errors.quantity?.message} required>
          <Input id="quantity" type="number" min={1} {...register('quantity')} />
        </FormField>
      </form>
    </Dialog>
  )
}
