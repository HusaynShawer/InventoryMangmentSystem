import { useEffect } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Dialog } from '@/components/ui/dialog'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { FormField } from '@/components/ui/form-field'
import { categoryHooks } from './hooks'
import type { Category } from '@/types'

const schema = z.object({ name: z.string().min(1, 'Name is required') })
type FormValues = z.infer<typeof schema>

interface Props {
  open: boolean
  onClose: () => void
  category?: Category | null
}

export function CategoryFormDialog({ open, onClose, category }: Props) {
  const createMutation = categoryHooks.useCreate()
  const updateMutation = categoryHooks.useUpdate()
  const isEdit = Boolean(category)

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<FormValues>({ resolver: zodResolver(schema) })

  useEffect(() => {
    if (open) reset({ name: category?.name ?? '' })
  }, [open, category, reset])

  const onSubmit = async (values: FormValues) => {
    if (isEdit && category) {
      await updateMutation.mutateAsync({ id: category.id ?? '', input: values })
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
      title={isEdit ? 'Edit category' : 'New category'}
      footer={
        <>
          <Button variant="outline" onClick={onClose} disabled={loading}>
            Cancel
          </Button>
          <Button type="submit" form="category-form" loading={loading}>
            {isEdit ? 'Save changes' : 'Create'}
          </Button>
        </>
      }
    >
      <form id="category-form" onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <FormField label="Name" htmlFor="name" error={errors.name?.message} required>
          <Input id="name" placeholder="e.g. Electronics" {...register('name')} />
        </FormField>
      </form>
    </Dialog>
  )
}
