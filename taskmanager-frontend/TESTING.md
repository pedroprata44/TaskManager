# Testing Guide - TaskManager Frontend

## Overview

O projeto frontend segue **Test-Driven Development (TDD)** com cobertura completa de testes usando:

- **Vitest** - Test runner rápido e otimizado para Vite
- **React Testing Library** - Testes de componentes focados no comportamento do usuário
- **MSW (Mock Service Worker)** - Mock de requisições HTTP
- **Happy DOM** - Ambiente DOM leve para testes

## Running Tests

### Run all tests
```bash
npm test
```

### Run tests in watch mode
```bash
npm test -- --watch
```

### Run tests with UI
```bash
npm test:ui
```

### Generate coverage report
```bash
npm test:coverage
```

## Project Structure

```
src/
├── test/
│   ├── setup.ts              # Test environment setup
│   ├── test-utils.tsx        # Custom render function with providers
│   └── mocks/
│       └── server.ts         # MSW mock handlers
├── services/
│   └── api.test.ts          # API client tests
├── store/
│   ├── authStore.test.ts    # Authentication store tests
│   └── taskStore.test.ts    # Task store tests
├── components/
│   ├── TaskForm.test.tsx
│   ├── TaskList.test.tsx
│   ├── TaskEditModal.test.tsx
│   └── ProtectedRoute.test.tsx
├── pages/
│   ├── Login.test.tsx
│   ├── Register.test.tsx
│   └── Dashboard.test.tsx
├── App.test.tsx             # App-level integration tests
└── test/
    └── integration.test.tsx # Complex integration scenarios
```

## Test Categories

### 1. Unit Tests - API Client (`src/services/api.test.ts`)
Tests the API client layer:
- ✅ Login with valid/invalid credentials
- ✅ User registration
- ✅ Token management
- ✅ Token header injection
- ✅ CRUD operations on tasks

### 2. Unit Tests - Store (`src/store/*.test.ts`)

#### Auth Store (`authStore.test.ts`)
- ✅ Initialization state
- ✅ Setting/clearing tokens
- ✅ localStorage persistence
- ✅ Token restoration from localStorage

#### Task Store (`taskStore.test.ts`)
- ✅ Fetching all tasks
- ✅ Creating new tasks
- ✅ Updating task data
- ✅ Updating task status
- ✅ Deleting tasks
- ✅ Error handling
- ✅ Loading states

### 3. Component Tests

#### Pages
- **Login** - Form submission, validation, error handling
- **Register** - Registration flow, password matching, duplicate user handling
- **Dashboard** - Task display, statistics, CRUD operations

#### Components
- **ProtectedRoute** - Auth guard behavior
- **TaskForm** - Task creation form
- **TaskList** - Task listing and interaction
- **TaskEditModal** - Task editing functionality

### 4. Integration Tests (`src/test/integration.test.tsx`)
- ✅ Complete user registration flow
- ✅ Complete user login flow
- ✅ Task CRUD operations with state management
- ✅ Multi-task scenarios
- ✅ Statistics update on task changes
- ✅ Error handling and display

### 5. App-Level Tests (`src/App.test.tsx`)
- ✅ Route protection
- ✅ Token persistence across app reload
- ✅ Logout flow
- ✅ End-to-end authentication scenarios

## Mock Data

### MSW Handlers (`src/test/mocks/server.ts`)

```typescript
// Valid credentials
POST /api/auth/login
- username: "validuser"
- password: "password123"
→ Returns JWT token

// Invalid credentials
- Any other credentials
→ Returns 401 Unauthorized

// User registration
POST /api/auth/register
- email, username, password
→ Returns token (fails if username "existinguser")

// Tasks endpoints
GET /api/tasks
→ Returns mock task list

POST /api/tasks (Create)
→ Creates new task

PUT /api/tasks/:id (Update)
→ Updates task

PATCH /api/tasks/:id (Update Status)
→ Updates task status

DELETE /api/tasks/:id (Delete)
→ Deletes task
```

## Test Utilities

### Custom Render Function (`src/test/test-utils.tsx`)

```typescript
import { render } from '@/test/test-utils'

// Wraps component with Router provider
render(<MyComponent />)
```

### Setup (`src/test/setup.ts`)

Provides:
- ✅ localStorage mock
- ✅ window.matchMedia mock
- ✅ MSW server setup
- ✅ Jest DOM matchers

## Writing New Tests

### Test Template
```typescript
import { describe, it, expect, beforeEach } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@/test/test-utils'
import MyComponent from '@/components/MyComponent'

describe('MyComponent', () => {
  beforeEach(() => {
    // Setup
  })

  it('should do something', async () => {
    render(<MyComponent />)
    
    const element = screen.getByRole('button')
    fireEvent.click(element)
    
    await waitFor(() => {
      expect(screen.getByText('Expected')).toBeInTheDocument()
    })
  })
})
```

## Coverage Goals

- **Statements**: > 80%
- **Branches**: > 75%
- **Functions**: > 80%
- **Lines**: > 80%

## Common Testing Patterns

### Testing Async Operations
```typescript
await act(async () => {
  await store.fetchTasks()
})
```

### Testing User Interactions
```typescript
const button = screen.getByRole('button', { name: /submit/i })
fireEvent.click(button)
```

### Testing Store Mutations
```typescript
const { result } = renderHook(() => useStore())
act(() => {
  result.current.setValue(newValue)
})
expect(result.current.value).toBe(newValue)
```

### Waiting for Elements
```typescript
await waitFor(() => {
  expect(screen.getByText('Success')).toBeInTheDocument()
})
```

## Best Practices

1. **Test User Behavior**: Focus on what users see and do
2. **Avoid Implementation Details**: Test behavior, not internal state
3. **Use Descriptive Names**: Test names should describe the scenario
4. **Keep Tests DRY**: Use beforeEach for common setup
5. **Mock External Dependencies**: Use MSW for API calls
6. **Test Error Cases**: Include negative test cases
7. **Use Accessibility Queries**: Prefer getByRole over getByTestId

## CI/CD Integration

Tests run automatically on:
- ✅ Every commit (pre-commit hook)
- ✅ Every pull request
- ✅ Before build

## Debugging Tests

### Run Single Test File
```bash
npm test -- src/services/api.test.ts
```

### Run Single Test
```bash
npm test -- -t "should login successfully"
```

### Debug in Browser
```bash
npm test:ui
```

## Performance

- All tests run in parallel
- Total test run: ~2-5 seconds
- Coverage report generation: ~3 seconds

## Troubleshooting

### localStorage is not defined
✅ Already mocked in `setup.ts`

### Tests timing out
- Increase timeout: `it('test', async () => {}, 10000)`
- Check MSW handlers

### Component not rendering
- Ensure Router provider is used
- Check useNavigate mocks

---

**Happy Testing! 🎉**
