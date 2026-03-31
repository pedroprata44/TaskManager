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

  it('should handle multiple tasks with different statuses', () => {
    useTaskStore.setState({
      tasks: [
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
      ],
    })

    render(<Dashboard />)

    expect(screen.getByText('Task 1')).toBeInTheDocument()
    expect(screen.getByText('Task 2')).toBeInTheDocument()
    expect(screen.getByText('Pending')).toBeInTheDocument()
    expect(screen.getByText('InProgress')).toBeInTheDocument()
  })

  it('should display statistics for different task statuses', () => {
    useTaskStore.setState({
      tasks: [
        {
          id: 'task-1',
          title: 'Task 1',
          description: '',
          status: TaskStatus.Pending,
          userId: 'user-1',
          createdAt: new Date().toISOString(),
        },
        {
          id: 'task-2',
          title: 'Task 2',
          description: '',
          status: TaskStatus.Pending,
          userId: 'user-1',
          createdAt: new Date().toISOString(),
        },
        {
          id: 'task-3',
          title: 'Task 3',
          description: '',
          status: TaskStatus.Completed,
          userId: 'user-1',
          createdAt: new Date().toISOString(),
        },
      ],
    })

    render(<Dashboard />)

    // Should display correct statistics
    expect(screen.getByText('Pendentes')).toBeInTheDocument()
    expect(screen.getByText('Em Progresso')).toBeInTheDocument()
    expect(screen.getByText('Concluídas')).toBeInTheDocument()
    expect(screen.getByText('2')).toBeInTheDocument() // 2 pending
    expect(screen.getByText('1')).toBeInTheDocument() // 1 completed
  })
})
