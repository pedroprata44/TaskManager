import { describe, it, expect, beforeEach } from 'vitest'
import { renderHook, act } from '@testing-library/react'
import { useAuthStore } from '@/store/authStore'

describe('Auth Store', () => {
  beforeEach(() => {
    localStorage.clear()
    useAuthStore.setState({ token: null, isAuthenticated: false })
  })

  it('should initialize with no token', () => {
    const { result } = renderHook(() => useAuthStore())

    expect(result.current.token).toBeNull()
    expect(result.current.isAuthenticated).toBe(false)
  })

  it('should set token and mark as authenticated', () => {
    const { result } = renderHook(() => useAuthStore())

    act(() => {
      result.current.setToken('test-token-123')
    })

    expect(result.current.token).toBe('test-token-123')
    expect(result.current.isAuthenticated).toBe(true)
    expect(localStorage.getItem('token')).toBe('test-token-123')
  })

  it('should clear token and mark as unauthenticated', () => {
    const { result } = renderHook(() => useAuthStore())

    act(() => {
      result.current.setToken('test-token-123')
    })

    expect(result.current.isAuthenticated).toBe(true)

    act(() => {
      result.current.clearToken()
    })

    expect(result.current.token).toBeNull()
    expect(result.current.isAuthenticated).toBe(false)
    expect(localStorage.getItem('token')).toBeNull()
  })

  it('should persist token to localStorage', () => {
    const { result } = renderHook(() => useAuthStore())

    act(() => {
      result.current.setToken('persistent-token')
    })

    expect(localStorage.getItem('token')).toBe('persistent-token')
  })

  it('should restore token from localStorage on initialization', () => {
    localStorage.setItem('token', 'restored-token')

    const { result } = renderHook(() => useAuthStore())

    expect(result.current.token).toBe('restored-token')
    expect(result.current.isAuthenticated).toBe(true)
  })
})
