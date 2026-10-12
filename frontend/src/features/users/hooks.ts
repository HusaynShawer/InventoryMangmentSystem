import { useMutation } from '@tanstack/react-query'
import { toast } from 'sonner'
import { registerRequest } from '@/api/auth'
import { extractErrorMessage } from '@/api/client'

/** Admin-only user creation (the API exposes register, but no user listing). */
export function useRegisterUser() {
  return useMutation({
    mutationFn: registerRequest,
    onSuccess: (data) => toast.success(`User ${data.userName} created`),
    onError: (error) => toast.error(extractErrorMessage(error)),
  })
}
