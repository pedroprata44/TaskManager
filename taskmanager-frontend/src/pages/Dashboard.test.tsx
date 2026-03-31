import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@/test/test-utils'
import { useNavigate } from 'react-router-dom'
import Dashboard from '@/pages/Dashboard'
import { useAuthStore } from '@/store/authStore'
import { useTaskStore } from '@/store/taskStore'
import { TaskStatus } from '@/types'

vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual('react-router-dom')
  return {
    ...actual,
    useNavigate: vi.fn(),
  }
})

describe('Dashboard Page', () => {
  const mockNavigate = vi.fn()

  beforeEach(() => {
    mockNavigate.mockClear()
    vi.mocked(useNavigate).mockReturnValue(mockNavigate)
    
    useAuthStore.setState({ token: 'test-token', isAuthenticated: true })
    useTaskStore.setState({
      tasks: [
        {
          id: 'task-1',
          title: 'Pending Task',
          description: 'A pending task',
          status: TaskStatus.Pending,
          userId: 'user-1',
          createdAt: new Date().toISOString(),
        },
        {
          id: 'task-2',
          title: 'In Progress Task',
          description: 'A task in progress',
          status: TaskStatus.InProgress,
          userId: 'user-1',
          createdAt: new Date().toISOString(),
        },
        {
          id: 'task-3',
          title: 'Completed Task',
          description: 'A completed task',
          status: TaskStatus.Completed,
          userId: 'user-1',
          createdAt: new Date().toISOString(),
        },
      ],
      loading: false,
      error: null,
    })
  })

  it('should render dashboard header', () => {
    render(<Dashboard />)

    expect(screen.getByText('TaskManager')).toBeInTheDocument()
    expect(screen.getByText(/sair/i)).toBeInTheDocument()
  })

  it('should display task statistics', () => {
    render(<Dashboard />)

    expect(screen.getByText('1')).toBeInTheDocument() // Pending
    expect(screen.getByText('1')).toBeInTheDocument() // In Progress
    expect(screen.getByText('1')).toBeInTheDocument() // Completed
  })

  it('should display stat labels', () => {
    render(<Dashboard />)

    expect(screen.getByText('Pendentes')).toBeInTheDocument()
    expect(screen.getByText('Em Progresso')).toBeInTheDocument()
    expect(screen.getByText('Concluídas')).toBeInTheDocument()
  })

  it('should render add task button', () => {
    render(<Dashboard />)

    expect(screen.getByRole('button', { name: /nova tarefa/i })).toBeInTheDocument()
  })

  it('should show task form when add task button is clicked', () => {
    render(<Dashboard />)

    const addButton = screen.getByRole('button', { name: /nova tarefa/i })
    fireEvent.click(addButton)

    expect(screen.getByText('Título')).toBeInTheDocument()
  })

  it('should display all tasks', () => {
    render(<Dashboard />)

    expect(screen.getByText('Pending Task')).toBeInTheDocument()
    expect(screen.getByText('In Progress Task')).toBeInTheDocument()
    expect(screen.getByText('Completed Task')).toBeInTheDocument()
  })

  it('should logout when logout button is clicked', () => {
    render(<Dashboard />)

    const logoutButton = screen.getByRole('button', { name: /sair/i })
    fireEvent.click(logoutButton)

    expect(mockNavigate).toHaveBeenCalledWith('/login')
  })

  it('should clear token on logout', async () => {
    render(<Dashboard />)

    const logoutButton = screen.getByRole('button', { name: /sair/i })
    fireEvent.click(logoutButton)

    await waitFor(() => {
      expect(useAuthStore.getState().token).toBeNull()
    })
  })

  it('should fetch tasks on mount', () => {
    const fetchTasksSpy = vi.spyOn(useTaskStore.getState(), 'fetchTasks')
    
    render(<Dashboard />)

    expect(fetchTasksSpy).toHaveBeenCalled()
  })

  it('should hide form when cancel is clicked', async () => {
    render(<Dashboard />)

    const addButton = screen.getByRole('button', { name: /nova tarefa/i })
    fireEvent.click(addButton)

    expect(screen.getByText('Título')).toBeInTheDocument()

    const cancelButton = screen.getByRole('button', { name: /cancelar/i })
    fireEvent.click(cancelButton)

    await waitFor(() => {
      expect(screen.queryByText('Título')).not.toBeInTheDocument()
    })
  })
})
