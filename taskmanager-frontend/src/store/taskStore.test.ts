import { describe, it, expect, beforeEach } from 'vitest'
import { renderHook, act } from '@testing-library/react'
import { useTaskStore } from '@/store/taskStore'
import { TaskStatus } from '@/types'

describe('Task Store', () => {
  beforeEach(() => {
    useTaskStore.setState({
      tasks: [],
      loading: false,
      error: null,
    })
  })

  it('should initialize with empty tasks', () => {
    const { result } = renderHook(() => useTaskStore())

    expect(result.current.tasks).toEqual([])
    expect(result.current.loading).toBe(false)
    expect(result.current.error).toBeNull()
  })

  it('should set error on failure', () => {
    const { result } = renderHook(() => useTaskStore())

    act(() => {
      result.current.setError('Test error')
    })

    expect(result.current.error).toBe('Test error')
  })

  it('should clear error on successful operation', () => {
    const { result } = renderHook(() => useTaskStore())

    act(() => {
      result.current.setError('Test error')
    })

    expect(result.current.error).toBe('Test error')

    act(() => {
      result.current.setError(null)
    })

    expect(result.current.error).toBeNull()
  })

  it('should allow setting tasks directly', () => {
    const { result } = renderHook(() => useTaskStore())

    const mockTasks = [
      {
        id: 'task-1',
        title: 'Task 1',
        description: 'Desc 1',
        status: TaskStatus.Todo,
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
    ]

    act(() => {
      useTaskStore.setState({
        tasks: mockTasks,
        loading: false,
      })
    })

    expect(result.current.tasks.length).toBe(2)
    expect(result.current.tasks[0].title).toBe('Task 1')
    expect(result.current.tasks[1].status).toBe(TaskStatus.InProgress)
  })
})
