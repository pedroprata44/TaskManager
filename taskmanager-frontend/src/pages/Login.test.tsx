import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@/test/test-utils'
import { useNavigate } from 'react-router-dom'
import Login from '@/pages/Login'

vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual('react-router-dom')
  return {
    ...actual,
    useNavigate: vi.fn(),
  }
})

describe('Login Page', () => {
  const mockNavigate = vi.fn()

  beforeEach(() => {
    localStorage.clear()
    vi.mocked(useNavigate).mockReturnValue(mockNavigate)
  })

  it('should render login form', () => {
    render(<Login />)

    expect(screen.getByText('Login')).toBeInTheDocument()
    expect(screen.getByText('TaskManager')).toBeInTheDocument()
    expect(screen.getByLabelText('Usuário')).toBeInTheDocument()
    expect(screen.getByLabelText('Senha')).toBeInTheDocument()
    expect(screen.getByRole('button', { name: /entrar/i })).toBeInTheDocument()
  })

  it('should have link to register page', async () => {
    render(<Login />)

    const registerLink = screen.getByRole('button', { name: /registrar/i })
    fireEvent.click(registerLink)

    await waitFor(() => {
      expect(mockNavigate).toHaveBeenCalledWith('/register')
    })
  })

  it('should accept input values', () => {
    render(<Login />)

    const usernameInput = screen.getByLabelText('Usuário') as HTMLInputElement
    const passwordInput = screen.getByLabelText('Senha') as HTMLInputElement

    fireEvent.change(usernameInput, { target: { value: 'testuser' } })
    fireEvent.change(passwordInput, { target: { value: 'password123' } })

    expect(usernameInput.value).toBe('testuser')
    expect(passwordInput.value).toBe('password123')
  })

  it('should submit form with credentials', async () => {
    render(<Login />)

    const usernameInput = screen.getByLabelText('Usuário')
    const passwordInput = screen.getByLabelText('Senha')
    const submitButton = screen.getByRole('button', { name: /entrar/i })

    fireEvent.change(usernameInput, { target: { value: 'validuser' } })
    fireEvent.change(passwordInput, { target: { value: 'password123' } })
    fireEvent.click(submitButton)

    await waitFor(() => {
      expect(mockNavigate).toHaveBeenCalledWith('/dashboard')
    })
  })

  it('should show loading state while submitting', async () => {
    render(<Login />)

    const usernameInput = screen.getByLabelText('Usuário')
    const passwordInput = screen.getByLabelText('Senha')
    const submitButton = screen.getByRole('button', { name: /entrar/i })

    fireEvent.change(usernameInput, { target: { value: 'validuser' } })
    fireEvent.change(passwordInput, { target: { value: 'password123' } })
    fireEvent.click(submitButton)

    expect(screen.getByRole('button', { name: /entrando/i })).toBeInTheDocument()
  })

  it('should display error on invalid credentials', async () => {
    render(<Login />)

    const usernameInput = screen.getByLabelText('Usuário')
    const passwordInput = screen.getByLabelText('Senha')
    const submitButton = screen.getByRole('button', { name: /entrar/i })

    fireEvent.change(usernameInput, { target: { value: 'invaliduser' } })
    fireEvent.change(passwordInput, { target: { value: 'wrongpassword' } })
    fireEvent.click(submitButton)

    await waitFor(() => {
      expect(screen.getByText(/erro ao fazer login/i)).toBeInTheDocument()
    })
  })
})
