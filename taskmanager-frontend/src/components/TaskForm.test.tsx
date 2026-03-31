import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@/test/test-utils'
import TaskForm from '@/components/TaskForm'
import { useTaskStore } from '@/store/taskStore'

describe('TaskForm', () => {
  const mockOnClose = vi.fn()

  beforeEach(() => {
    mockOnClose.mockClear()
    useTaskStore.setState({
      tasks: [],
      loading: false,
      error: null,
    })
  })

  it('should render form fields', () => {
    render(<TaskForm onClose={mockOnClose} />)

    expect(screen.getByLabelText('Título')).toBeInTheDocument()
    expect(screen.getByLabelText('Descrição')).toBeInTheDocument()
    expect(screen.getByRole('button', { name: /criar tarefa/i })).toBeInTheDocument()
    expect(screen.getByRole('button', { name: /cancelar/i })).toBeInTheDocument()
  })

  it('should accept input values', () => {
    render(<TaskForm onClose={mockOnClose} />)

    const titleInput = screen.getByLabelText('Título') as HTMLInputElement
    const descriptionInput = screen.getByLabelText(
      'Descrição'
    ) as HTMLTextAreaElement

    fireEvent.change(titleInput, { target: { value: 'Test Task' } })
    fireEvent.change(descriptionInput, { target: { value: 'Task description' } })

    expect(titleInput.value).toBe('Test Task')
    expect(descriptionInput.value).toBe('Task description')
  })

  it('should call onClose when cancel button is clicked', () => {
    render(<TaskForm onClose={mockOnClose} />)

    const cancelButton = screen.getByRole('button', { name: /cancelar/i })
    fireEvent.click(cancelButton)

    expect(mockOnClose).toHaveBeenCalled()
  })

  it('should submit form with task data', async () => {
    render(<TaskForm onClose={mockOnClose} />)

    const titleInput = screen.getByLabelText('Título')
    const descriptionInput = screen.getByLabelText('Descrição')
    const submitButton = screen.getByRole('button', { name: /criar tarefa/i })

    fireEvent.change(titleInput, { target: { value: 'New Task' } })
    fireEvent.change(descriptionInput, { target: { value: 'Task desc' } })
    fireEvent.click(submitButton)

    await waitFor(() => {
      expect(mockOnClose).toHaveBeenCalled()
    })
  })

  it('should show loading state while submitting', async () => {
    render(<TaskForm onClose={mockOnClose} />)

    const titleInput = screen.getByLabelText('Título')
    const submitButton = screen.getByRole('button', { name: /criar tarefa/i })

    fireEvent.change(titleInput, { target: { value: 'Test Task' } })
    fireEvent.click(submitButton)

    expect(screen.getByRole('button', { name: /criando/i })).toBeInTheDocument()
  })

  it('should require title field', async () => {
    render(<TaskForm onClose={mockOnClose} />)

    const titleInput = screen.getByLabelText('Título') as HTMLInputElement

    expect(titleInput.required).toBe(true)
  })
})
