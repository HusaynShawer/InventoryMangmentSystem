import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { toast } from 'sonner'
import { stockApi } from '@/api/stock'
import { stockMovementsApi } from '@/api/stockMovements'
import { extractErrorMessage } from '@/api/client'
import { queryKeys } from '@/lib/constants'

export function useStockList() {
  return useQuery({ queryKey: queryKeys.stock, queryFn: stockApi.list })
}

export function useStockMovements() {
  return useQuery({ queryKey: queryKeys.stockMovements, queryFn: stockMovementsApi.list })
}

function useStockInvalidate() {
  const queryClient = useQueryClient()
  return () => {
    queryClient.invalidateQueries({ queryKey: queryKeys.stock })
    queryClient.invalidateQueries({ queryKey: queryKeys.stockMovements })
  }
}

export function useCreateStock() {
  const invalidate = useStockInvalidate()
  return useMutation({
    mutationFn: stockApi.create,
    onSuccess: () => {
      invalidate()
      toast.success('Stock record created')
    },
    onError: (error) => toast.error(extractErrorMessage(error)),
  })
}

export function useDeleteStock() {
  const invalidate = useStockInvalidate()
  return useMutation({
    mutationFn: stockApi.remove,
    onSuccess: () => {
      invalidate()
      toast.success('Stock record deleted')
    },
    onError: (error) => toast.error(extractErrorMessage(error)),
  })
}

export function useAdjustStock() {
  const invalidate = useStockInvalidate()
  return useMutation({
    mutationFn: stockApi.adjust,
    onSuccess: () => {
      invalidate()
      toast.success('Stock adjusted')
    },
    onError: (error) => toast.error(extractErrorMessage(error)),
  })
}

export function useTransferStock() {
  const invalidate = useStockInvalidate()
  return useMutation({
    mutationFn: stockApi.transfer,
    onSuccess: () => {
      invalidate()
      toast.success('Stock transferred')
    },
    onError: (error) => toast.error(extractErrorMessage(error)),
  })
}

export function useCreateStockMovement() {
  const invalidate = useStockInvalidate()
  return useMutation({
    mutationFn: stockMovementsApi.create,
    onSuccess: () => {
      invalidate()
      toast.success('Stock movement recorded')
    },
    onError: (error) => toast.error(extractErrorMessage(error)),
  })
}

export function useDeleteStockMovement() {
  const invalidate = useStockInvalidate()
  return useMutation({
    mutationFn: stockMovementsApi.remove,
    onSuccess: () => {
      invalidate()
      toast.success('Stock movement deleted')
    },
    onError: (error) => toast.error(extractErrorMessage(error)),
  })
}
