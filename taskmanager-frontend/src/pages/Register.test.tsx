import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@/test/test-utils'
import { useNavigate } from 'react-router-dom'
import Register from '@/pages/Register'

vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual('react-router-dom')
  return {
    ...actual,
    useNavigate: vi.fn(),
  }
})

describe('Register Page', () => {
  const mockNavigate = vi.fn()

  beforeEach(() => {
    localStorage.clear()
    vi.mocked(useNavigate).mockReturnValue(mockNavigate)
  })

  it('should render registration form', () => {
    render(<Register />)

    expect(screen.getByText('Registre-se')).toBeInTheDocument()
    expect(screen.getByLabelText('Email')).toBeInTheDocument()
    expect(screen.getByLabelText('Usuário')).toBeInTheDocument()
    expect(screen.getAllByLabelText('Senha')[0]).toBeInTheDocument()
    expect(screen.getByLabelText('Confirmar Senha')).toBeInTheDocument()
  })

  it('should accept input values', () => {
    render(<Register />)

    const emailInput = screen.getByLabelText('Email') as HTMLInputElement
    const usernameInput = screen.getByLabelText('Usuário') as HTMLInputElement
    const passwordInputs = screen.getAllByLabelText('Senha')
    const confirmPasswordInput = screen.getByLabelText(
      'Confirmar Senha'
    ) as HTMLInputElement

    fireEvent.change(emailInput, { target: { value: 'test@example.com' } })
    fireEvent.change(usernameInput, { target: { value: 'testuser' } })
    fireEvent.change(passwordInputs[0], { target: { value: 'password123' } })
    fireEvent.change(confirmPasswordInput, { target: { value: 'password123' } })

    expect(emailInput.value).toBe('test@example.com')
    expect(usernameInput.value).toBe('testuser')
    expect(confirmPasswordInput.value).toBe('password123')
  })

  it('should show error if passwords do not match', async () => {
    render(<Register />)

    const emailInput = screen.getByLabelText('Email')
    const usernameInput = screen.getByLabelText('Usuário')
    const passwordInputs = screen.getAllByLabelText('Senha')
    const confirmPasswordInput = screen.getByLabelText('Confirmar Senha')
    const submitButton = screen.getByRole('button', { name: /registrar/i })

    fireEvent.change(emailInput, { target: { value: 'test@example.com' } })
    fireEvent.change(usernameInput, { target: { value: 'testuser' } })
    fireEvent.change(passwordInputs[0], { target: { value: 'password123' } })
    fireEvent.change(confirmPasswordInput, { target: { value: 'different' } })
    fireEvent.click(submitButton)

    await waitFor(() => {
      expect(
        screen.getByText('As senhas não correspondem')
      ).toBeInTheDocument()
    })
  })

  it('should successfully register with valid data', async () => {
    render(<Register />)

    const emailInput = screen.getByLabelText('Email')
    const usernameInput = screen.getByLabelText('Usuário')
    const passwordInputs = screen.getAllByLabelText('Senha')
    const confirmPasswordInput = screen.getByLabelText('Confirmar Senha')
    const submitButton = screen.getByRole('button', { name: /registrar/i })

    fireEvent.change(emailInput, { target: { value: 'newuser@example.com' } })
    fireEvent.change(usernameInput, { target: { value: 'newuser' } })
    fireEvent.change(passwordInputs[0], { target: { value: 'password123' } })
    fireEvent.change(confirmPasswordInput, { target: { value: 'password123' } })
    fireEvent.click(submitButton)

    await waitFor(() => {
      expect(mockNavigate).toHaveBeenCalledWith('/dashboard')
    })
  })

  it('should have link to login page', async () => {
    render(<Register />)

    const loginLink = screen.getByRole('button', { name: /fazer login/i })
    fireEvent.click(loginLink)

    await waitFor(() => {
      expect(mockNavigate).toHaveBeenCalledWith('/login')
    })
  })
})
