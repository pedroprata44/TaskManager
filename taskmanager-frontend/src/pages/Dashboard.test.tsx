import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen, fireEvent } from '@/test/test-utils'
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
      ],
      loading: false,
      error: null,
      fetchTasks: vi.fn(async () => {
        // Mock fetchTasks to do nothing
      }),
      addTask: vi.fn(),
      updateTask: vi.fn(),
      updateTaskStatus: vi.fn(),
      deleteTask: vi.fn(),
      setError: vi.fn(),
    })
  })

  it('should render dashboard header', () => {
    render(<Dashboard />)
    expect(screen.getByText('TaskManager')).toBeInTheDocument()
  })

  it('should display stat labels', () => {
    render(<Dashboard />)
    expect(screen.getByText('Pendentes')).toBeInTheDocument()
    expect(screen.getByText('Em Progresso')).toBeInTheDocument()
    expect(screen.getByText('Concluídas')).toBeInTheDocument()
  })

  it('should display add task button', () => {
    render(<Dashboard />)
    expect(screen.getByText(/Nova Tarefa/i)).toBeInTheDocument()
  })

  it('should display all tasks', () => {
    render(<Dashboard />)
    expect(screen.getByText('Task 1')).toBeInTheDocument()
    expect(screen.getByText('Task 2')).toBeInTheDocument()
  })

  it('should logout when logout button is clicked', () => {
    render(<Dashboard />)
    const logoutButton = screen.getByText(/Sair/i)
    fireEvent.click(logoutButton)
    expect(mockNavigate).toHaveBeenCalledWith('/login')
  })
})
