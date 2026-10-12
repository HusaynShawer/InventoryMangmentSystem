import { useMutation, useQuery, useQueryClient, type QueryKey } from '@tanstack/react-query'
import { toast } from 'sonner'
import { extractErrorMessage } from '@/api/client'

/**
 * Minimal shape every resource wrapper implements. A factory keeps the
 * per-feature hooks files tiny and consistent (same cache keys, toasts, errors).
 */
export interface ResourceApi<T, TInput> {
  list: () => Promise<T[]>
  create: (input: TInput) => Promise<unknown>
  update: (id: string, input: TInput) => Promise<unknown>
  remove: (id: string) => Promise<unknown>
}

export function createCrudHooks<T, TInput>(
  key: QueryKey,
  resource: ResourceApi<T, TInput>,
  label: string,
) {
  function useList() {
    return useQuery({ queryKey: key, queryFn: resource.list })
  }

  function useCreate() {
    const queryClient = useQueryClient()
    return useMutation({
      mutationFn: (input: TInput) => resource.create(input),
      onSuccess: () => {
        queryClient.invalidateQueries({ queryKey: key })
        toast.success(`${label} created`)
      },
      onError: (error) => toast.error(extractErrorMessage(error)),
    })
  }

  function useUpdate() {
    const queryClient = useQueryClient()
    return useMutation({
      mutationFn: ({ id, input }: { id: string; input: TInput }) => resource.update(id, input),
      onSuccess: () => {
        queryClient.invalidateQueries({ queryKey: key })
        toast.success(`${label} updated`)
      },
      onError: (error) => toast.error(extractErrorMessage(error)),
    })
  }

  function useDelete() {
    const queryClient = useQueryClient()
    return useMutation({
      mutationFn: (id: string) => resource.remove(id),
      onSuccess: () => {
        queryClient.invalidateQueries({ queryKey: key })
        toast.success(`${label} deleted`)
      },
      onError: (error) => toast.error(extractErrorMessage(error)),
    })
  }

  return { useList, useCreate, useUpdate, useDelete }
}
