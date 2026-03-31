import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen } from '@/test/test-utils'
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

  it('should render registration form heading', () => {
    render(<Register />)
    expect(screen.getByText(/Registre-se/i)).toBeInTheDocument()
  })

  it('should have email input field', () => {
    render(<Register />)
    const inputs = screen.getAllByPlaceholderText(/email/i)
    expect(inputs.length).toBeGreaterThan(0)
  })

  it('should have username input field', () => {
    render(<Register />)
    const inputs = screen.getAllByPlaceholderText(/usuário|username/i)
    expect(inputs.length).toBeGreaterThan(0)
  })

  it('should have password input fields', () => {
    render(<Register />)
    const inputs = screen.getAllByPlaceholderText(/senha|password/i)
    expect(inputs.length).toBeGreaterThanOrEqual(2)
  })

  it('should have submit button', () => {
    render(<Register />)
    const button = screen.getByRole('button', { name: /registrar/i })
    expect(button).toBeInTheDocument()
  })

  it('should have login link', () => {
    render(<Register />)
    const link = screen.getByText(/fazer login/i)
    expect(link).toBeInTheDocument()
  })
})
