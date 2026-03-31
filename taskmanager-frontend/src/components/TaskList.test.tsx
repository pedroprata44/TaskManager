import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@/test/test-utils'
import TaskList from '@/components/TaskList'
import { useTaskStore } from '@/store/taskStore'
import { TaskItem, TaskStatus } from '@/types'

describe('TaskList', () => {
  const mockTasks: TaskItem[] = [
    {
      id: 'task-1',
      title: 'Task 1',
      description: 'Description 1',
      status: TaskStatus.Pending,
      userId: 'user-1',
      createdAt: new Date().toISOString(),
    },
    {
      id: 'task-2',
      title: 'Task 2',
      description: 'Description 2',
      status: TaskStatus.InProgress,
      userId: 'user-1',
      createdAt: new Date().toISOString(),
    },
  ]

  beforeEach(() => {
    useTaskStore.setState({
      tasks: mockTasks,
      loading: false,
      error: null,
    })
  })

  it('should render task list', () => {
    render(<TaskList tasks={mockTasks} />)

    expect(screen.getByText('Task 1')).toBeInTheDocument()
    expect(screen.getByText('Task 2')).toBeInTheDocument()
  })

  it('should display task descriptions', () => {
    render(<TaskList tasks={mockTasks} />)

    expect(screen.getByText('Description 1')).toBeInTheDocument()
    expect(screen.getByText('Description 2')).toBeInTheDocument()
  })

  it('should display task status badges', () => {
    render(<TaskList tasks={mockTasks} />)

    expect(screen.getByText('Pending')).toBeInTheDocument()
    expect(screen.getByText('InProgress')).toBeInTheDocument()
  })

  it('should have edit button for each task', () => {
    render(<TaskList tasks={mockTasks} />)

    const editButtons = screen.getAllByTitle('Editar')
    expect(editButtons).toHaveLength(mockTasks.length)
  })

  it('should have delete button for each task', () => {
    render(<TaskList tasks={mockTasks} />)

    const deleteButtons = screen.getAllByTitle('Deletar')
    expect(deleteButtons).toHaveLength(mockTasks.length)
  })

  it('should have status toggle button for each task', () => {
    render(<TaskList tasks={mockTasks} />)

    const statusButtons = screen.getAllByTitle('Mudar status')
    expect(statusButtons).toHaveLength(mockTasks.length)
  })

  it('should call delete when delete button is clicked', async () => {
    const deleteTaskSpy = vi.spyOn(useTaskStore.getState(), 'deleteTask')

    render(<TaskList tasks={mockTasks} />)

    const deleteButtons = screen.getAllByTitle('Deletar')
    fireEvent.click(deleteButtons[0])

    await waitFor(() => {
      expect(deleteTaskSpy).toHaveBeenCalled()
    })
  })

  it('should show edit modal when edit button is clicked', async () => {
    render(<TaskList tasks={mockTasks} />)

    const editButtons = screen.getAllByTitle('Editar')
    fireEvent.click(editButtons[0])

    await waitFor(() => {
      expect(screen.getByText('Editar Tarefa')).toBeInTheDocument()
    })
  })
})
