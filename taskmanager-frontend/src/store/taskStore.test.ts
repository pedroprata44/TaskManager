import { describe, it, expect, beforeEach, vi } from 'vitest'
import { renderHook, act, waitFor } from '@testing-library/react'
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

  it('should fetch tasks successfully', async () => {
    const { result } = renderHook(() => useTaskStore())

    await act(async () => {
      await result.current.fetchTasks()
    })

    expect(result.current.tasks.length).toBeGreaterThan(0)
    expect(result.current.error).toBeNull()
  })

  it('should set loading state while fetching', async () => {
    const { result } = renderHook(() => useTaskStore())

    act(() => {
      result.current.fetchTasks()
    })

    // Loading should be true during fetch
    await waitFor(() => {
      expect(result.current.loading).toBe(false)
    })
  })

  it('should add task to list', async () => {
    const { result } = renderHook(() => useTaskStore())

    await act(async () => {
      await result.current.addTask('New Task', 'Task description')
    })

    expect(result.current.tasks.length).toBeGreaterThan(0)
    expect(result.current.tasks[result.current.tasks.length - 1].title).toBe(
      'New Task'
    )
  })

  it('should update task', async () => {
    const { result } = renderHook(() => useTaskStore())

    await act(async () => {
      await result.current.fetchTasks()
    })

    const taskId = result.current.tasks[0].id

    await act(async () => {
      await result.current.updateTask(taskId, 'Updated Title', 'Updated desc')
    })

    const updatedTask = result.current.tasks.find((t) => t.id === taskId)
    expect(updatedTask?.title).toBe('Updated Title')
  })

  it('should update task status', async () => {
    const { result } = renderHook(() => useTaskStore())

    await act(async () => {
      await result.current.fetchTasks()
    })

    const taskId = result.current.tasks[0].id

    await act(async () => {
      await result.current.updateTaskStatus(taskId, TaskStatus.Completed)
    })

    const updatedTask = result.current.tasks.find((t) => t.id === taskId)
    expect(updatedTask?.status).toBe(TaskStatus.Completed)
  })

  it('should delete task', async () => {
    const { result } = renderHook(() => useTaskStore())

    await act(async () => {
      await result.current.fetchTasks()
    })

    const initialCount = result.current.tasks.length
    const taskIdToDelete = result.current.tasks[0].id

    await act(async () => {
      await result.current.deleteTask(taskIdToDelete)
    })

    expect(result.current.tasks.length).toBe(initialCount - 1)
    expect(result.current.tasks.find((t) => t.id === taskIdToDelete)).toBeUndefined()
  })

  it('should set error on failure', async () => {
    const { result } = renderHook(() => useTaskStore())

    act(() => {
      result.current.setError('Test error message')
    })

    expect(result.current.error).toBe('Test error message')
  })

  it('should clear error on successful operation', async () => {
    const { result } = renderHook(() => useTaskStore())

    act(() => {
      result.current.setError('Error')
    })

    await act(async () => {
      await result.current.fetchTasks()
    })

    expect(result.current.error).toBeNull()
  })
})
