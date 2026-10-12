import { api, getList } from './client'
import type { Category, CategoryInput } from '@/types'

export const categoriesApi = {
  list: () => getList<Category>('/Category'),
  get: (id: string) => api.get<Category>(`/Category/${id}`).then((r) => r.data),
  create: (input: CategoryInput) => api.post<Category>('/Category', input).then((r) => r.data),
  update: (id: string, input: CategoryInput) =>
    api.put(`/Category/${id}`, input).then((r) => r.data),
  remove: (id: string) => api.delete(`/Category/${id}`).then((r) => r.data),
}
