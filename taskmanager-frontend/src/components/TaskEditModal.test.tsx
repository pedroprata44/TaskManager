import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@/test/test-utils'
import TaskEditModal from '@/components/TaskEditModal'
import { useTaskStore } from '@/store/taskStore'
import { TaskItem, TaskStatus } from '@/types'

describe('TaskEditModal', () => {
  const mockTask: TaskItem = {
    id: 'task-1',
    title: 'Task 1',
    description: 'Description 1',
    status: TaskStatus.Pending,
    userId: 'user-1',
    createdAt: new Date().toISOString(),
  }

  const mockOnClose = vi.fn()

  beforeEach(() => {
    mockOnClose.mockClear()
    useTaskStore.setState({
      tasks: [mockTask],
      loading: false,
      error: null,
    })
  })

  it('should render modal with task data', () => {
    render(<TaskEditModal task={mockTask} onClose={mockOnClose} />)

    expect(screen.getByText('Editar Tarefa')).toBeInTheDocument()
    expect(screen.getByDisplayValue('Task 1')).toBeInTheDocument()
    expect(screen.getByDisplayValue('Description 1')).toBeInTheDocument()
  })

  it('should have close button', () => {
    render(<TaskEditModal task={mockTask} onClose={mockOnClose} />)

    const closeButtons = screen.getAllByRole('button').filter(
      (btn) => btn.querySelector('svg') !== null
    )
    fireEvent.click(closeButtons[0])

    expect(mockOnClose).toHaveBeenCalled()
  })

  it('should allow editing title', () => {
    render(<TaskEditModal task={mockTask} onClose={mockOnClose} />)

    const titleInput = screen.getByDisplayValue('Task 1') as HTMLInputElement
    fireEvent.change(titleInput, { target: { value: 'Updated Title' } })

    expect(titleInput.value).toBe('Updated Title')
  })

  it('should allow editing description', () => {
    render(<TaskEditModal task={mockTask} onClose={mockOnClose} />)

    const descriptionInput = screen.getByDisplayValue(
      'Description 1'
    ) as HTMLTextAreaElement
    fireEvent.change(descriptionInput, {
      target: { value: 'Updated Description' },
    })

    expect(descriptionInput.value).toBe('Updated Description')
  })

  it('should submit form with updated data', async () => {
    render(<TaskEditModal task={mockTask} onClose={mockOnClose} />)

    const titleInput = screen.getByDisplayValue('Task 1')
    const submitButton = screen.getByRole('button', { name: /atualizar/i })

    fireEvent.change(titleInput, { target: { value: 'Updated Task' } })
    fireEvent.click(submitButton)

    await waitFor(() => {
      expect(mockOnClose).toHaveBeenCalled()
    })
  })

  it('should show loading state while submitting', async () => {
    render(<TaskEditModal task={mockTask} onClose={mockOnClose} />)

    const submitButton = screen.getByRole('button', { name: /atualizar/i })
    fireEvent.click(submitButton)

    expect(
      screen.getByRole('button', { name: /atualizando/i })
    ).toBeInTheDocument()
  })

  it('should call onClose when cancel button is clicked', () => {
    render(<TaskEditModal task={mockTask} onClose={mockOnClose} />)

    const cancelButton = screen.getByRole('button', { name: /cancelar/i })
    fireEvent.click(cancelButton)

    expect(mockOnClose).toHaveBeenCalled()
  })
})
