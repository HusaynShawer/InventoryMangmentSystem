import { api, getList } from './client'
import type { ProductAttribute, ProductAttributeInput, ProductAttributeValueInput } from '@/types'

export const productAttributesApi = {
  list: () => getList<ProductAttribute>('/product-attributes'),
  get: (id: string) => api.get<ProductAttribute>(`/product-attributes/${id}`).then((r) => r.data),
  create: (input: ProductAttributeInput) => api.post<ProductAttribute>('/product-attributes', input).then((r) => r.data),
  update: (id: string, input: ProductAttributeInput) => api.put<ProductAttribute>(`/product-attributes/${id}`, input).then((r) => r.data),
  remove: (id: string) => api.delete(`/product-attributes/${id}`).then((r) => r.data),
  addValue: (id: string, input: ProductAttributeValueInput) => api.post(`/product-attributes/${id}/values`, input).then((r) => r.data),
  removeValue: (id: string, valueId: string) => api.delete(`/product-attributes/${id}/values/${valueId}`).then((r) => r.data),
}
