import { api } from './client'
import type { LoginDto, RegisterDto } from '@/types'

export interface AuthResponse {
  token: string
  userName: string
  role: string
  expiresAt: string
}

export async function loginRequest(payload: LoginDto): Promise<AuthResponse> {
  const { data } = await api.post<AuthResponse>('/Auth/login', payload)
  return data
}

/** Admin-only: create a new user account. */
export async function registerRequest(payload: RegisterDto): Promise<AuthResponse> {
  const { data } = await api.post<AuthResponse>('/Auth/register', payload)
  return data
}
