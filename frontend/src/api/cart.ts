import { api, getList } from './client'
import type { AddToCartInput, Cart, CartItem } from '@/types'

export const cartApi = {
  get: () => api.get<Cart>('/Cart').then((r) => r.data),
  items: () => getList<CartItem>('/Cart/items'),
  add: (input: AddToCartInput) => api.post('/Cart/add', input).then((r) => r.data),
  updateItem: (itemId: string, quantity: number) =>
    api.put(`/Cart/items/${itemId}`, { quantity }).then((r) => r.data),
  removeItem: (itemId: string) => api.delete(`/Cart/items/${itemId}`).then((r) => r.data),
  clear: () => api.delete('/Cart/clear').then((r) => r.data),
}
