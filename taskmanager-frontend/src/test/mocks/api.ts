import { vi } from 'vitest'
import { TaskItem, TaskStatus, AuthResponse } from '@/types'

export const apiClientMock = {
  login: vi.fn(async (payload: any): Promise<AuthResponse> => ({
    token: 'mock-token',
    expiresIn: 3600,
    user: {
      id: 'user-1',
      username: payload.username,
      email: 'user@example.com',
    },
  })),
  
  register: vi.fn(async (payload: any): Promise<AuthResponse> => ({
    token: 'mock-token-new',
    expiresIn: 3600,
    user: {
      id: 'user-new',
      username: payload.username,
      email: payload.email,
    },
  })),
  
  getTasks: vi.fn(async (): Promise<TaskItem[]> => [
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
  ]),
  
  getTask: vi.fn(async (id: string): Promise<TaskItem> => ({
    id,
    title: 'Task 1',
    description: 'Description 1',
    status: TaskStatus.Pending,
    userId: 'user-1',
    createdAt: new Date().toISOString(),
  })),
  
  createTask: vi.fn(async (payload: any): Promise<TaskItem> => ({
    id: 'task-new',
    title: payload.title,
    description: payload.description || '',
    status: payload.status || TaskStatus.Pending,
    userId: 'user-1',
    createdAt: new Date().toISOString(),
  })),
  
  updateTask: vi.fn(async (id: string, payload: any): Promise<TaskItem> => ({
    id,
    title: payload.title || 'Task 1',
    description: payload.description || 'Description 1',
    status: TaskStatus.Pending,
    userId: 'user-1',
    createdAt: new Date().toISOString(),
  })),
  
  updateTaskStatus: vi.fn(async (id: string, status: string): Promise<TaskItem> => ({
    id,
    title: 'Task 1',
    description: 'Description 1',
    status: status as TaskStatus,
    userId: 'user-1',
    createdAt: new Date().toISOString(),
  })),
  
  deleteTask: vi.fn(async (): Promise<void> => {}),
}

// Export as apiClient for compatibility with imports
export const apiClient = apiClientMock
export default apiClient
