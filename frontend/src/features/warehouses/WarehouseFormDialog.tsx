import { useEffect } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Dialog } from '@/components/ui/dialog'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { FormField } from '@/components/ui/form-field'
import { warehouseHooks } from './hooks'
import type { Warehouse } from '@/types'

const schema = z.object({
  name: z.string().min(1, 'Name is required'),
  location: z.string().min(1, 'Location is required'),
})
type FormValues = z.infer<typeof schema>

interface Props {
  open: boolean
  onClose: () => void
  warehouse?: Warehouse | null
}

export function WarehouseFormDialog({ open, onClose, warehouse }: Props) {
  const createMutation = warehouseHooks.useCreate()
  const updateMutation = warehouseHooks.useUpdate()
  const isEdit = Boolean(warehouse)

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<FormValues>({ resolver: zodResolver(schema) })

  useEffect(() => {
    if (open) reset({ name: warehouse?.name ?? '', location: warehouse?.location ?? '' })
  }, [open, warehouse, reset])

  const onSubmit = async (values: FormValues) => {
    if (isEdit && warehouse) {
      await updateMutation.mutateAsync({ id: warehouse.id ?? '', input: values })
    } else {
      await createMutation.mutateAsync(values)
    }
    onClose()
  }

  const loading = createMutation.isPending || updateMutation.isPending

  return (
    <Dialog
      open={open}
      onClose={onClose}
      title={isEdit ? 'Edit warehouse' : 'New warehouse'}
      footer={
        <>
          <Button variant="outline" onClick={onClose} disabled={loading}>
            Cancel
          </Button>
          <Button type="submit" form="warehouse-form" loading={loading}>
            {isEdit ? 'Save changes' : 'Create'}
          </Button>
        </>
      }
    >
      <form id="warehouse-form" onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <FormField label="Name" htmlFor="name" error={errors.name?.message} required>
          <Input id="name" placeholder="Main Warehouse" {...register('name')} />
        </FormField>
        <FormField label="Location" htmlFor="location" error={errors.location?.message} required>
          <Input id="location" placeholder="City / address" {...register('location')} />
        </FormField>
      </form>
    </Dialog>
  )
}
