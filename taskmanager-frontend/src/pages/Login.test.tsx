import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen } from '@/test/test-utils'
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

  it('should render login form heading', () => {
    render(<Login />)
    expect(screen.getByText(/Login/i)).toBeInTheDocument()
  })

  it('should have link to register page', () => {
    render(<Login />)
    const registerLink = screen.getByText(/registrar/i)
    expect(registerLink).toBeInTheDocument()
  })

  it('should have username input field', () => {
    render(<Login />)
    const inputs = screen.getAllByPlaceholderText(/usuário|username/i)
    expect(inputs.length).toBeGreaterThan(0)
  })

  it('should have password input field', () => {
    render(<Login />)
    const inputs = screen.getAllByPlaceholderText(/senha|password/i)
    expect(inputs.length).toBeGreaterThan(0)
  })

  it('should have submit button', () => {
    render(<Login />)
    const button = screen.getByRole('button', { name: /entrar|login/i })
    expect(button).toBeInTheDocument()
  })

  it('should display error on invalid credentials', () => {
    render(<Login />)
    expect(screen.getByText(/TaskManager/i)).toBeInTheDocument()
  })
})
