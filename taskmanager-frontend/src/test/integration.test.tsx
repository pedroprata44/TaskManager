import { describe, it, expect, beforeEach } from 'vitest'
import { render, screen } from '@/test/test-utils'
import Dashboard from '@/pages/Dashboard'
import { useAuthStore } from '@/store/authStore'
import { useTaskStore } from '@/store/taskStore'
import { TaskStatus } from '@/types'

describe('Task Management Integration', () => {
  beforeEach(() => {
    useAuthStore.setState({ token: 'test-token', isAuthenticated: true })
    useTaskStore.setState({
      tasks: [],
      loading: false,
      error: null,
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

  it('should display statistics when tasks exist', () => {
    const mockTasks = [
      {
        id: 'task-1',
        title: 'Task 1',
        description: 'Desc 1',
        status: TaskStatus.Pending,
        userId: 'user-1',
        createdAt: new Date().toISOString(),
      },
      {
        id: 'task-2',
        title: 'Task 2',
        description: 'Desc 2',
        status: TaskStatus.InProgress,
        userId: 'user-1',
        createdAt: new Date().toISOString(),
      },
      {
        id: 'task-3',
        title: 'Task 3',
        description: 'Desc 3',
        status: TaskStatus.Completed,
        userId: 'user-1',
        createdAt: new Date().toISOString(),
      },
    ]

    // Set tasks directly to store to avoid async fetch
    useTaskStore.setState({
      tasks: mockTasks,
      loading: false,
      error: null,
    })

    render(<Dashboard />)

    // Check that stat sections are displayed
    expect(screen.getByText('Pendentes')).toBeInTheDocument()
    expect(screen.getByText('Em Progresso')).toBeInTheDocument()
    expect(screen.getByText('Concluídas')).toBeInTheDocument()
  })

  it('should display logout button', () => {
    render(<Dashboard />)
    expect(screen.getByText('Sair')).toBeInTheDocument()
  })

  it('should display add task button', () => {
    render(<Dashboard />)
    expect(screen.getByText('+ Nova Tarefa')).toBeInTheDocument()
  })
})
