import { useEffect } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Dialog } from '@/components/ui/dialog'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Select } from '@/components/ui/select'
import { FormField } from '@/components/ui/form-field'
import { supplierHooks } from '@/features/suppliers/hooks'
import { ROLES, ROLE_LABELS } from '@/lib/constants'
import { useRegisterUser } from './hooks'

const schema = z.object({
  userName: z.string().min(3, 'At least 3 characters').max(50),
  email: z.string().email('Enter a valid email'),
  password: z.string().min(6, 'At least 6 characters'),
  fullName: z.string().max(100).optional(),
  role: z.enum(['ADMIN', 'Supplier', 'User']),
  supplierId: z.string().optional(),
})
type FormValues = z.infer<typeof schema>

export function UserCreateDialog({ open, onClose }: { open: boolean; onClose: () => void }) {
  const registerUser = useRegisterUser()
  const { data: suppliers = [] } = supplierHooks.useList()

  const {
    register,
    handleSubmit,
    reset,
    watch,
    formState: { errors },
  } = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: { userName: '', email: '', password: '', fullName: '', role: 'User', supplierId: '' },
  })

  const role = watch('role')

  useEffect(() => {
    if (open) reset({ userName: '', email: '', password: '', fullName: '', role: 'User', supplierId: '' })
  }, [open, reset])

  const onSubmit = async (values: FormValues) => {
    await registerUser.mutateAsync({
      userName: values.userName,
      email: values.email,
      password: values.password,
      fullName: values.fullName,
      role: values.role,
      // SupplierId is only meaningful for supplier accounts.
      supplierId: values.role === 'Supplier' && values.supplierId ? values.supplierId : undefined,
    })
    onClose()
  }

  return (
    <Dialog
      open={open}
      onClose={onClose}
      title="Create user"
      description="Accounts are created through the register endpoint (admin only)."
      footer={
        <>
          <Button variant="outline" onClick={onClose} disabled={registerUser.isPending}>
            Cancel
          </Button>
          <Button type="submit" form="user-form" loading={registerUser.isPending}>
            Create user
          </Button>
        </>
      }
    >
      <form id="user-form" onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <div className="grid gap-4 sm:grid-cols-2">
          <FormField label="Username" htmlFor="userName" error={errors.userName?.message} required>
            <Input id="userName" {...register('userName')} />
          </FormField>
          <FormField label="Full name" htmlFor="fullName" error={errors.fullName?.message}>
            <Input id="fullName" {...register('fullName')} />
          </FormField>
        </div>
        <FormField label="Email" htmlFor="email" error={errors.email?.message} required>
          <Input id="email" type="email" {...register('email')} />
        </FormField>
        <FormField label="Password" htmlFor="password" error={errors.password?.message} required>
          <Input id="password" type="password" {...register('password')} />
        </FormField>
        <FormField label="Role" htmlFor="role" error={errors.role?.message} required>
          <Select id="role" {...register('role')}>
            {ROLES.map((value) => (
              <option key={value} value={value}>
                {ROLE_LABELS[value]}
              </option>
            ))}
          </Select>
        </FormField>
        {role === 'Supplier' && (
          <FormField label="Supplier" htmlFor="supplierId" error={errors.supplierId?.message}>
            <Select id="supplierId" {...register('supplierId')}>
              <option value="">No linked supplier</option>
              {suppliers.map((supplier) => (
                <option key={supplier.id} value={supplier.id}>
                  {supplier.name}
                </option>
              ))}
            </Select>
          </FormField>
        )}
      </form>
    </Dialog>
  )
}
