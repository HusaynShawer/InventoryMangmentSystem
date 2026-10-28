import { api, getList } from './client'
import type { ProductVariant, CreateProductVariantInput } from '@/types'

export const productVariantsApi = {
  listByProduct: (productId: string) => getList<ProductVariant>(`/products/${productId}/variants`),
  get: (productId: string, id: string) => api.get<ProductVariant>(`/products/${productId}/variants/${id}`).then((r) => r.data),
  create: (productId: string, input: CreateProductVariantInput) =>
    api.post<ProductVariant>(`/products/${productId}/variants`, input).then((r) => r.data),
  update: (productId: string, id: string, input: CreateProductVariantInput) =>
    api.put<ProductVariant>(`/products/${productId}/variants/${id}`, input).then((r) => r.data),
  remove: (productId: string, id: string) => api.delete(`/products/${productId}/variants/${id}`).then((r) => r.data),
}
