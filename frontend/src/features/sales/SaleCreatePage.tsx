import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useNavigate } from 'react-router-dom'
import { ArrowLeft, Save } from 'lucide-react'
import { PageHeader } from '@/components/data/PageHeader'
import { LineItemsEditor } from '@/components/data/LineItemsEditor'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Select } from '@/components/ui/select'
import { FormField } from '@/components/ui/form-field'
import { warehouseHooks } from '@/features/warehouses/hooks'
import { productHooks } from '@/features/products/hooks'
import { useAuth } from '@/features/auth/useAuth'
import { formatCurrency } from '@/lib/utils'
import { paths } from '@/routes/paths'
import { saleHooks } from './hooks'
import type { SaleInput } from '@/types'

const schema = z.object({
  warehouseId: z.string().uuid('Select a warehouse'),
  date: z.string().optional(),
  items: z
    .array(
      z.object({
        productId: z.string().uuid('Select a product'),
        quantity: z.coerce.number().int().positive('Must be at least 1'),
        unitPrice: z.coerce.number().min(0, 'Cannot be negative'),
      }),
    )
    .min(1, 'Add at least one line item'),
})
type FormValues = z.infer<typeof schema>

export function SaleCreatePage() {
  const navigate = useNavigate()
  const { user } = useAuth()
  const createMutation = saleHooks.useCreate()
  const { data: warehouses = [] } = warehouseHooks.useList()
  const { data: products = [] } = productHooks.useList()

  const {
    register,
    handleSubmit,
    control,
    setValue,
    watch,
    formState: { errors },
  } = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: {
      warehouseId: '',
      date: '',
      items: [{ productId: '', quantity: 1, unitPrice: 0 }],
    },
  })

  const items = watch('items')
  const total = (items ?? []).reduce(
    (sum, item) => sum + Number(item?.quantity ?? 0) * Number(item?.unitPrice ?? 0),
    0,
  )

  const onSubmit = async (values: FormValues) => {
    const payload: SaleInput = {
      warehouseId: values.warehouseId,
      createdByUserID: user?.userId ?? undefined,
      date: values.date ? new Date(values.date).toISOString() : undefined,
      totalAmount: total,
      items: values.items.map((item) => ({
        productId: item.productId,
        quantity: item.quantity,
        unitPrice: item.unitPrice,
      })),
    }
    await createMutation.mutateAsync(payload)
    navigate(paths.sales)
  }

  return (
    <>
      <PageHeader
        title="New sale"
        description="Stock is deducted from the selected warehouse once saved."
        actions={
          <Button variant="outline" onClick={() => navigate(paths.sales)}>
            <ArrowLeft className="h-4 w-4" />
            Back
          </Button>
        }
      />

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
        <div className="grid gap-4 rounded-lg border border-border bg-card p-5 sm:grid-cols-2">
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
          <FormField label="Date" htmlFor="date" error={errors.date?.message} hint="Defaults to now">
            <Input id="date" type="datetime-local" {...register('date')} />
          </FormField>
        </div>

        <div className="rounded-lg border border-border bg-card p-5">
          <h2 className="mb-4 text-sm font-semibold">Line items</h2>
          <LineItemsEditor
            control={control}
            register={register}
            setValue={setValue}
            errors={errors}
            products={products}
          />

          <div className="mt-5 flex items-center justify-end gap-4 border-t border-border pt-4">
            <span className="text-sm text-muted-foreground">Total</span>
            <span className="text-xl font-semibold">{formatCurrency(total)}</span>
          </div>
        </div>

        <div className="flex justify-end gap-2">
          <Button type="button" variant="outline" onClick={() => navigate(paths.sales)}>
            Cancel
          </Button>
          <Button type="submit" loading={createMutation.isPending}>
            <Save className="h-4 w-4" />
            Save sale
          </Button>
        </div>
      </form>
    </>
  )
}
