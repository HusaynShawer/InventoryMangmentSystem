import { useEffect } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Dialog } from '@/components/ui/dialog'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { FormField } from '@/components/ui/form-field'
import { supplierHooks } from './hooks'
import type { Supplier } from '@/types'

const schema = z.object({
  name: z.string().min(1, 'Name is required'),
  contactInfo: z.string().min(1, 'Contact info is required'),
})
type FormValues = z.infer<typeof schema>

interface Props {
  open: boolean
  onClose: () => void
  supplier?: Supplier | null
}

export function SupplierFormDialog({ open, onClose, supplier }: Props) {
  const createMutation = supplierHooks.useCreate()
  const updateMutation = supplierHooks.useUpdate()
  const isEdit = Boolean(supplier)

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<FormValues>({ resolver: zodResolver(schema) })

  useEffect(() => {
    if (open) reset({ name: supplier?.name ?? '', contactInfo: supplier?.contactInfo ?? '' })
  }, [open, supplier, reset])

  const onSubmit = async (values: FormValues) => {
    if (isEdit && supplier) {
      await updateMutation.mutateAsync({ id: supplier.id!, input: values })
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
      title={isEdit ? 'Edit supplier' : 'New supplier'}
      footer={
        <>
          <Button variant="outline" onClick={onClose} disabled={loading}>
            Cancel
          </Button>
          <Button type="submit" form="supplier-form" loading={loading}>
            {isEdit ? 'Save changes' : 'Create'}
          </Button>
        </>
      }
    >
      <form id="supplier-form" onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <FormField label="Name" htmlFor="name" error={errors.name?.message} required>
          <Input id="name" placeholder="Acme Supplies" {...register('name')} />
        </FormField>
        <FormField
          label="Contact info"
          htmlFor="contactInfo"
          error={errors.contactInfo?.message}
          required
        >
          <Input id="contactInfo" placeholder="email / phone / address" {...register('contactInfo')} />
        </FormField>
      </form>
    </Dialog>
  )
}
