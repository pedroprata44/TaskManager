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
  })

  it('should persist token to localStorage', () => {
    const { result } = renderHook(() => useAuthStore())

    act(() => {
      result.current.setToken('persistent-token')
    })

    // Zustand persist middleware stores state
    expect(result.current.token).toBe('persistent-token')
    expect(result.current.isAuthenticated).toBe(true)
  })

  it('should toggle authentication state', () => {
    const { result } = renderHook(() => useAuthStore())

    // Start unauthenticated
    expect(result.current.isAuthenticated).toBe(false)

    act(() => {
      result.current.setToken('auth-token')
    })

    expect(result.current.isAuthenticated).toBe(true)

    act(() => {
      result.current.clearToken()
    })

    expect(result.current.isAuthenticated).toBe(false)
  })
})
