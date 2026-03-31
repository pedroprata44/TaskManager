import { setupServer } from 'msw/node'
import { http, HttpResponse } from 'msw'

const API_BASE_URL = 'https://localhost:5001/api'

export const handlers = [
  // Auth endpoints
  http.post(`${API_BASE_URL}/auth/login`, async ({ request }) => {
    const body = await request.json() as { username: string; password: string }
    
    if (body.username === 'validuser' && body.password === 'password123') {
      return HttpResponse.json({
        token: 'mock-jwt-token-123',
        expiresIn: 3600,
        user: {
          id: 'user-1',
          username: 'validuser',
          email: 'user@example.com',
        },
      })
    }

    return HttpResponse.json(
      { message: 'Invalid credentials' },
      { status: 401 }
    )
  }),

  http.post(`${API_BASE_URL}/auth/register`, async ({ request }) => {
    const body = await request.json() as { 
      email: string
      username: string
      password: string 
    }

    if (body.username === 'existinguser') {
      return HttpResponse.json(
        { message: 'User already exists' },
        { status: 409 }
      )
    }

    return HttpResponse.json({
      token: 'mock-jwt-token-new-user',
      expiresIn: 3600,
      user: {
        id: 'user-new',
        username: body.username,
        email: body.email,
      },
    })
  }),

  // Task endpoints
  http.get(`${API_BASE_URL}/tasks`, () => {
    return HttpResponse.json([
      {
        id: 'task-1',
        title: 'Task 1',
        description: 'Description 1',
        status: 'Pending',
        userId: 'user-1',
        createdAt: new Date().toISOString(),
      },
      {
        id: 'task-2',
        title: 'Task 2',
        description: 'Description 2',
        status: 'InProgress',
        userId: 'user-1',
        createdAt: new Date().toISOString(),
      },
    ])
  }),

  http.post(`${API_BASE_URL}/tasks`, async ({ request }) => {
    const body = await request.json() as { title: string; description?: string }

    return HttpResponse.json({
      id: 'task-new',
      title: body.title,
      description: body.description || '',
      status: 'Pending',
      userId: 'user-1',
      createdAt: new Date().toISOString(),
    })
  }),

  http.put(`${API_BASE_URL}/tasks/:id`, async ({ request }) => {
    const body = await request.json() as { title?: string; description?: string }

    return HttpResponse.json({
      id: 'task-1',
      title: body.title || 'Task 1',
      description: body.description || 'Description 1',
      status: 'Pending',
      userId: 'user-1',
      createdAt: new Date().toISOString(),
    })
  }),

  http.patch(`${API_BASE_URL}/tasks/:id`, async ({ request }) => {
    const body = await request.json() as { status: string }

    return HttpResponse.json({
      id: 'task-1',
      title: 'Task 1',
      description: 'Description 1',
      status: body.status,
      userId: 'user-1',
      createdAt: new Date().toISOString(),
    })
  }),

  http.delete(`${API_BASE_URL}/tasks/:id`, () => {
    return HttpResponse.json({}, { status: 204 })
  }),
]

export const server = setupServer(...handlers)
