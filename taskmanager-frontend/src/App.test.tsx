import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@/test/test-utils'
import { useNavigate } from 'react-router-dom'
import App from '@/App'
import { useAuthStore } from '@/store/authStore'

vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual('react-router-dom')
  return {
    ...actual,
    useNavigate: vi.fn(),
  }
})

describe('App Integration Tests', () => {
  beforeEach(() => {
    localStorage.clear()
    useAuthStore.setState({ token: null, isAuthenticated: false })
  })

  it('should redirect unauthenticated users to login', () => {
    render(<App />)

    expect(screen.getByText('Login')).toBeInTheDocument()
  })

  it('should allow user registration flow', async () => {
    render(<App />)

    // Navigate to register
    const registerLink = screen.getByRole('button', { name: /registrar/i })
    fireEvent.click(registerLink)

    await waitFor(() => {
      expect(screen.getByText('Registre-se')).toBeInTheDocument()
    })

    // Fill registration form
    const emailInput = screen.getByLabelText('Email')
    const usernameInput = screen.getByLabelText('Usuário')
    const passwordInputs = screen.getAllByLabelText('Senha')
    const confirmPasswordInput = screen.getByLabelText('Confirmar Senha')

    fireEvent.change(emailInput, { target: { value: 'newuser@example.com' } })
    fireEvent.change(usernameInput, { target: { value: 'newuser' } })
    fireEvent.change(passwordInputs[0], { target: { value: 'password123' } })
    fireEvent.change(confirmPasswordInput, { target: { value: 'password123' } })

    const submitButton = screen.getByRole('button', { name: /registrar/i })
    fireEvent.click(submitButton)

    // Should navigate to dashboard
    await waitFor(() => {
      expect(localStorage.getItem('token')).toBe('mock-jwt-token-new-user')
    })
  })

  it('should allow user login flow', async () => {
    render(<App />)

    // Fill login form
    const usernameInput = screen.getByLabelText('Usuário')
    const passwordInput = screen.getByLabelText('Senha')

    fireEvent.change(usernameInput, { target: { value: 'validuser' } })
    fireEvent.change(passwordInput, { target: { value: 'password123' } })

    const submitButton = screen.getByRole('button', { name: /entrar/i })
    fireEvent.click(submitButton)

    // Should store token
    await waitFor(() => {
      expect(localStorage.getItem('token')).toBe('mock-jwt-token-123')
    })
  })

  it('should persist token across app reload', () => {
    const initialToken = 'persistent-token'
    localStorage.setItem('token', initialToken)

    useAuthStore.setState({ token: initialToken, isAuthenticated: true })

    const { rerender } = render(<App />)

    expect(localStorage.getItem('token')).toBe(initialToken)

    rerender(<App />)

    expect(localStorage.getItem('token')).toBe(initialToken)
  })

  it('should logout user and clear token', async () => {
    localStorage.setItem('token', 'existing-token')
    useAuthStore.setState({ token: 'existing-token', isAuthenticated: true })

    render(<App />)

    // Should show dashboard
    await waitFor(() => {
      expect(screen.getByText('TaskManager')).toBeInTheDocument()
    })

    // Logout
    const logoutButton = screen.getByRole('button', { name: /sair/i })
    fireEvent.click(logoutButton)

    // Should return to login
    await waitFor(() => {
      expect(screen.getByText('Login')).toBeInTheDocument()
    })

    expect(localStorage.getItem('token')).toBeNull()
  })
})
