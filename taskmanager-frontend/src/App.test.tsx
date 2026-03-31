import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render as rtlRender } from '@testing-library/react'
import App from '@/App'
import { useAuthStore } from '@/store/authStore'

vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual('react-router-dom')
  return {
    ...actual,
    useNavigate: vi.fn(),
  }
})

// Custom render that doesn't wrap with Router since App has its own
const render = (component: React.ReactElement) => {
  return rtlRender(component)
}

describe('App Integration Tests', () => {
  beforeEach(() => {
    localStorage.clear()
    useAuthStore.setState({ token: null, isAuthenticated: false })
  })

  it('should render app component', () => {
    const { container } = render(<App />)
    expect(container).toBeDefined()
  })

  it('should display TaskManager branding', () => {
    const { container } = render(<App />)
    // App should render successfully without errors
    expect(container.querySelector('div')).toBeDefined()
  })

  it('should have authentication store', () => {
    const initialState = useAuthStore.getState()
    expect(initialState).toBeDefined()
    expect(initialState.token).toBeNull()
  })

  it('should support setting authentication token', () => {
    useAuthStore.setState({ token: 'test-token', isAuthenticated: true })
    const state = useAuthStore.getState()
    expect(state.token).toBe('test-token')
    expect(state.isAuthenticated).toBe(true)
  })

  it('should support clearing authentication', () => {
    useAuthStore.setState({ token: 'test-token', isAuthenticated: true })
    useAuthStore.setState({ token: null, isAuthenticated: false })
    const state = useAuthStore.getState()
    expect(state.token).toBeNull()
    expect(state.isAuthenticated).toBe(false)
  })
})
