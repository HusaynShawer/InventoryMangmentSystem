import { useMutation, useQueryClient } from '@tanstack/react-query'
import { toast } from 'sonner'
import { registerRequest } from '@/api/auth'
import { extractErrorMessage } from '@/api/client'
import { userWarehousesApi, type UserWarehouseInput } from '@/api/userWarehouses'
import { queryKeys } from '@/lib/constants'

/** Admin-only user creation (the API exposes register, but no user listing). */
export function useRegisterUser() {
  return useMutation({
    mutationFn: registerRequest,
    onSuccess: (data) => toast.success(`User ${data.userName} created`),
    onError: (error) => toast.error(extractErrorMessage(error)),
  })
}

/** Admin-only: grant a user access to a warehouse. */
export function useAssignWarehouse() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (input: UserWarehouseInput) => userWarehousesApi.create(input),
    onSuccess: () => {
      toast.success('Warehouse assigned')
      queryClient.invalidateQueries({ queryKey: queryKeys.userWarehouses })
    },
    onError: (error) => toast.error(extractErrorMessage(error)),
  })
}
