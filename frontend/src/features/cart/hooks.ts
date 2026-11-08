import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { toast } from 'sonner'
import { cartApi } from '@/api/cart'
import { extractErrorMessage } from '@/api/client'
import { queryKeys } from '@/lib/constants'
import type { AddToCartInput } from '@/types'

export function useCart() {
  return useQuery({ queryKey: queryKeys.cart, queryFn: cartApi.get })
}

function useCartInvalidate() {
  const queryClient = useQueryClient()
  return () => {
    queryClient.invalidateQueries({ queryKey: queryKeys.cart })
    // Reserving/releasing stock changes the available quantities shown elsewhere.
    queryClient.invalidateQueries({ queryKey: queryKeys.stock })
  }
}

export function useAddToCart() {
  const invalidate = useCartInvalidate()
  return useMutation({
    mutationFn: (input: AddToCartInput) => cartApi.add(input),
    onSuccess: () => {
      invalidate()
      toast.success('Added to cart')
    },
    onError: (error) => toast.error(extractErrorMessage(error)),
  })
}

export function useUpdateCartItem() {
  const invalidate = useCartInvalidate()
  return useMutation({
    mutationFn: ({ itemId, quantity }: { itemId: string; quantity: number }) =>
      cartApi.updateItem(itemId, quantity),
    onSuccess: () => invalidate(),
    onError: (error) => toast.error(extractErrorMessage(error)),
  })
}

export function useRemoveCartItem() {
  const invalidate = useCartInvalidate()
  return useMutation({
    mutationFn: (itemId: string) => cartApi.removeItem(itemId),
    onSuccess: () => {
      invalidate()
      toast.success('Removed from cart')
    },
    onError: (error) => toast.error(extractErrorMessage(error)),
  })
}

export function useClearCart() {
  const invalidate = useCartInvalidate()
  return useMutation({
    mutationFn: () => cartApi.clear(),
    onSuccess: () => {
      invalidate()
      toast.success('Cart cleared')
    },
    onError: (error) => toast.error(extractErrorMessage(error)),
  })
}
