import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useLocation, useNavigate } from 'react-router-dom'
import { Boxes, LogIn } from 'lucide-react'
import { toast } from 'sonner'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { FormField } from '@/components/ui/form-field'
import { extractErrorMessage } from '@/api/client'
import { useAuth } from './useAuth'

const schema = z.object({
  userName: z.string().min(1, 'Username is required'),
  password: z.string().min(1, 'Password is required'),
})

type FormValues = z.infer<typeof schema>

export function LoginPage() {
  const { login } = useAuth()
  const navigate = useNavigate()
  const location = useLocation()
  const from = (location.state as { from?: { pathname: string } } | null)?.from?.pathname ?? '/'

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<FormValues>({ resolver: zodResolver(schema), defaultValues: { userName: '', password: '' } })

  const onSubmit = async (values: FormValues) => {
    try {
      const user = await login(values)
      toast.success(`Welcome back, ${user.userName}`)
      navigate(from, { replace: true })
    } catch (error) {
      toast.error(extractErrorMessage(error))
    }
  }

  return (
    <div className="flex min-h-screen items-center justify-center bg-background p-4">
      <div className="w-full max-w-sm">
        <div className="mb-8 flex flex-col items-center gap-2 text-center">
          <div className="flex h-12 w-12 items-center justify-center rounded-xl bg-primary text-primary-foreground">
            <Boxes className="h-6 w-6" />
          </div>
          <h1 className="text-xl font-semibold">Inventory Management</h1>
          <p className="text-sm text-muted-foreground">Sign in to manage your stock</p>
        </div>

        <form
          onSubmit={handleSubmit(onSubmit)}
          className="space-y-4 rounded-lg border border-border bg-card p-6 shadow-sm"
        >
          <FormField label="Username" htmlFor="userName" error={errors.userName?.message} required>
            <Input id="userName" autoComplete="username" placeholder="admin" {...register('userName')} />
          </FormField>
          <FormField label="Password" htmlFor="password" error={errors.password?.message} required>
            <Input
              id="password"
              type="password"
              autoComplete="current-password"
              placeholder="••••••••"
              {...register('password')}
            />
          </FormField>
          <Button type="submit" className="w-full" loading={isSubmitting}>
            <LogIn className="h-4 w-4" />
            Sign in
          </Button>
        </form>
      </div>
    </div>
  )
}
