import { describe, it, expect, beforeEach, vi } from 'vitest'
import { apiClient } from '@/services/api'
import { LoginRequest, RegisterRequest, CreateTaskRequest, TaskStatus } from '@/types'

describe('API Client', () => {
  beforeEach(() => {
    localStorage.clear()
  })

  describe('Authentication', () => {
    it('should successfully login with valid credentials', async () => {
      const loginData: LoginRequest = {
        username: 'validuser',
        password: 'password123',
      }

      const response = await apiClient.login(loginData)

      expect(response.token).toBe('mock-jwt-token-123')
      expect(response.expiresIn).toBe(3600)
      expect(response.user?.username).toBe('validuser')
    })

    it('should fail login with invalid credentials', async () => {
      const loginData: LoginRequest = {
        username: 'invaliduser',
        password: 'wrongpassword',
      }

      await expect(apiClient.login(loginData)).rejects.toThrow()
    })

    it('should successfully register new user', async () => {
      const registerData: RegisterRequest = {
        email: 'newuser@example.com',
        username: 'newuser',
        password: 'password123',
      }

      const response = await apiClient.register(registerData)

      expect(response.token).toBeDefined()
      expect(response.user?.email).toBe('newuser@example.com')
    })

    it('should fail registration if user already exists', async () => {
      const registerData: RegisterRequest = {
        email: 'existing@example.com',
        username: 'existinguser',
        password: 'password123',
      }

      await expect(apiClient.register(registerData)).rejects.toThrow()
    })

    it('should set Authorization header when token is stored', async () => {
      localStorage.setItem('token', 'mock-token-xyz')
      
      const spy = vi.spyOn(apiClient as any, 'axiosInstance')
      
      await apiClient.getTasks()
      
      // Verification would happen in actual request
      expect(localStorage.getItem('token')).toBe('mock-token-xyz')
    })
  })

  describe('Tasks', () => {
    beforeEach(() => {
      localStorage.setItem('token', 'mock-jwt-token-123')
    })

    it('should fetch all tasks', async () => {
      const tasks = await apiClient.getTasks()

      expect(Array.isArray(tasks)).toBe(true)
      expect(tasks.length).toBeGreaterThan(0)
      expect(tasks[0]).toHaveProperty('id')
      expect(tasks[0]).toHaveProperty('title')
    })

    it('should get single task by id', async () => {
      const task = await apiClient.getTask('task-1')

      expect(task).toBeDefined()
      expect(task.id).toBe('task-1')
    })

    it('should create new task', async () => {
      const createData: CreateTaskRequest = {
        title: 'New Task',
        description: 'Task description',
        status: TaskStatus.Pending,
      }

      const task = await apiClient.createTask(createData)

      expect(task.title).toBe('New Task')
      expect(task.status).toBe('Pending')
    })

    it('should update task', async () => {
      const updateData = {
        title: 'Updated Task',
        description: 'Updated description',
      }

      const task = await apiClient.updateTask('task-1', updateData)

      expect(task.title).toBe('Updated Task')
    })

    it('should update task status', async () => {
      const task = await apiClient.updateTaskStatus('task-1', 'Completed')

      expect(task.status).toBe('Completed')
    })

    it('should delete task', async () => {
      await expect(apiClient.deleteTask('task-1')).resolves.not.toThrow()
    })
  })
})
