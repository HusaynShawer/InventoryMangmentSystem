import { useEffect } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useQuery } from '@tanstack/react-query'
import { Dialog } from '@/components/ui/dialog'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Select } from '@/components/ui/select'
import { FormField } from '@/components/ui/form-field'
import { warehousesApi } from '@/api/warehouses'
import { queryKeys } from '@/lib/constants'
import { useAssignWarehouse } from './hooks'

const ACCESS_LEVELS = ['Admin', 'Manager', 'Staff'] as const

const schema = z.object({
  userId: z.string().uuid('Enter a valid user ID'),
  warehouseId: z.string().uuid('Select a warehouse'),
  accessLevel: z.enum(ACCESS_LEVELS),
})
type FormValues = z.infer<typeof schema>

interface Props {
  open: boolean
  onClose: () => void
  defaultUserId?: string
}

/** Grants a user access to a warehouse so they can create variants and stock. */
export function AssignWarehouseDialog({ open, onClose, defaultUserId }: Props) {
  const assign = useAssignWarehouse()
  const { data: warehouses = [] } = useQuery({
    queryKey: queryKeys.warehouses,
    queryFn: warehousesApi.list,
    enabled: open,
  })

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<FormValues>({ resolver: zodResolver(schema) })

  useEffect(() => {
    if (open) reset({ userId: defaultUserId ?? '', warehouseId: '', accessLevel: 'Staff' })
  }, [open, defaultUserId, reset])

  const onSubmit = async (values: FormValues) => {
    await assign.mutateAsync({
      userId: values.userId,
      warehouseId: values.warehouseId,
      accessLevel: values.accessLevel,
    })
    onClose()
  }

  return (
    <Dialog
      open={open}
      onClose={onClose}
      title="Assign warehouse"
      description="Users can only create variants and stock for warehouses assigned to them."
      footer={
        <>
          <Button variant="outline" onClick={onClose} disabled={assign.isPending}>
            Cancel
          </Button>
          <Button type="submit" form="assign-warehouse-form" loading={assign.isPending}>
            Assign
          </Button>
        </>
      }
    >
      <form id="assign-warehouse-form" onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <FormField label="User ID" htmlFor="userId" error={errors.userId?.message} required>
          <Input
            id="userId"
            placeholder="00000000-0000-0000-0000-000000000000"
            {...register('userId')}
          />
        </FormField>
        <FormField
          label="Warehouse"
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
        <FormField label="Access level" htmlFor="accessLevel" error={errors.accessLevel?.message}>
          <Select id="accessLevel" {...register('accessLevel')}>
            {ACCESS_LEVELS.map((level) => (
              <option key={level} value={level}>
                {level}
              </option>
            ))}
          </Select>
        </FormField>
      </form>
    </Dialog>
  )
}
