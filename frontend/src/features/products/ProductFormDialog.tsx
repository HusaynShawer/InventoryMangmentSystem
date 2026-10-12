import { useEffect } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Dialog } from '@/components/ui/dialog'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Select } from '@/components/ui/select'
import { FormField } from '@/components/ui/form-field'
import { categoryHooks } from '@/features/categories/hooks'
import { warehouseHooks } from '@/features/warehouses/hooks'
import { productHooks } from './hooks'
import type { Product, ProductInput } from '@/types'

const schema = z.object({
  sku: z.string().min(1, 'SKU is required'),
  name: z.string().min(1, 'Name is required'),
  categoryId: z.string().uuid('Select a category'),
  unitPrice: z.coerce.number().min(0, 'Must be zero or more'),
  reorderLevel: z.coerce.number().int().min(0, 'Must be zero or more'),
  // Only used when creating: opening stock for a warehouse.
  warehouseId: z.string().optional(),
  quantity: z.coerce.number().int().min(0).optional(),
})
type FormValues = z.infer<typeof schema>

interface Props {
  open: boolean
  onClose: () => void
  product?: Product | null
}

export function ProductFormDialog({ open, onClose, product }: Props) {
  const createMutation = productHooks.useCreate()
  const updateMutation = productHooks.useUpdate()
  const { data: categories = [] } = categoryHooks.useList()
  const { data: warehouses = [] } = warehouseHooks.useList()
  const isEdit = Boolean(product)

  const {
    register,
    handleSubmit,
    reset,
    setError,
    formState: { errors },
  } = useForm<FormValues>({ resolver: zodResolver(schema) })

  useEffect(() => {
    if (!open) return
    reset({
      sku: product?.sku ?? '',
      name: product?.name ?? '',
      categoryId: product?.categoryId ?? categories[0]?.id ?? '',
      unitPrice: product?.unitPrice ?? 0,
      reorderLevel: product?.reorderLevel ?? 0,
      warehouseId: warehouses[0]?.id ?? '',
      quantity: 0,
    })
  }, [open, product, categories, warehouses, reset])

  const onSubmit = async (values: FormValues) => {
    const payload: ProductInput = {
      sku: values.sku,
      name: values.name,
      categoryId: values.categoryId,
      unitPrice: values.unitPrice,
      reorderLevel: values.reorderLevel,
    }

    if (!isEdit) {
      // The create endpoint dereferences stock, so it must always be present.
      if (!values.warehouseId) {
        setError('warehouseId', { message: 'Choose a warehouse for opening stock' })
        return
      }
      payload.stock = { warehouseId: values.warehouseId, quantity: values.quantity ?? 0 }
      await createMutation.mutateAsync(payload)
    } else if (product) {
      await updateMutation.mutateAsync({ id: product.id!, input: payload })
    }
    onClose()
  }

  const loading = createMutation.isPending || updateMutation.isPending

  return (
    <Dialog
      open={open}
      onClose={onClose}
      title={isEdit ? 'Edit product' : 'New product'}
      className="max-w-xl"
      footer={
        <>
          <Button variant="outline" onClick={onClose} disabled={loading}>
            Cancel
          </Button>
          <Button type="submit" form="product-form" loading={loading}>
            {isEdit ? 'Save changes' : 'Create'}
          </Button>
        </>
      }
    >
      <form id="product-form" onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <div className="grid gap-4 sm:grid-cols-2">
          <FormField label="SKU" htmlFor="sku" error={errors.sku?.message} required>
            <Input id="sku" placeholder="SKU-001" {...register('sku')} />
          </FormField>
          <FormField label="Name" htmlFor="name" error={errors.name?.message} required>
            <Input id="name" placeholder="Product name" {...register('name')} />
          </FormField>
        </div>

        <FormField label="Category" htmlFor="categoryId" error={errors.categoryId?.message} required>
          <Select id="categoryId" {...register('categoryId')}>
            <option value="">Select a category…</option>
            {categories.map((category) => (
              <option key={category.id} value={category.id}>
                {category.name}
              </option>
            ))}
          </Select>
        </FormField>

        <div className="grid gap-4 sm:grid-cols-2">
          <FormField label="Unit price" htmlFor="unitPrice" error={errors.unitPrice?.message} required>
            <Input id="unitPrice" type="number" step="0.01" min={0} {...register('unitPrice')} />
          </FormField>
          <FormField
            label="Reorder level"
            htmlFor="reorderLevel"
            error={errors.reorderLevel?.message}
            required
            hint="Low-stock warnings trigger at or below this quantity."
          >
            <Input id="reorderLevel" type="number" min={0} {...register('reorderLevel')} />
          </FormField>
        </div>

        {!isEdit && (
          <div className="grid gap-4 rounded-md border border-border bg-muted/30 p-4 sm:grid-cols-2">
            <FormField
              label="Opening warehouse"
              htmlFor="warehouseId"
              error={errors.warehouseId?.message}
              required
            >
              <Select id="warehouseId" {...register('warehouseId')}>
                <option value="">Select a warehouse…</option>
                {warehouses.map((warehouse) => (
                  <option key={warehouse.id} value={warehouse.id}>
                    {warehouse.name}
                  </option>
                ))}
              </Select>
            </FormField>
            <FormField label="Opening quantity" htmlFor="quantity" error={errors.quantity?.message}>
              <Input id="quantity" type="number" min={0} {...register('quantity')} />
            </FormField>
          </div>
        )}
      </form>
    </Dialog>
  )
}
