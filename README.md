# TaskManager

## Description

A modern full-stack task management application with ASP.NET Core 10.0 backend and React 18 frontend. Features JWT authentication, multi-user support with task ownership, comprehensive test coverage (99 passing tests), and a responsive user interface.

## 🚀 Quick Start (Local)

**Pré-requisitos:** .NET 10 e Node.js 18+

### Init
```bash
# Backend
cd TaskManager/TaskManager.Api
dotnet restore
dotnet ef database update
dotnet run

# Frontend (em outro terminal)
cd TaskManager/taskmanager-frontend
npm install
npm run dev
```

## Features

- **JWT Authentication**: Secure token-based authentication with register and login endpoints.
- **Multi-User Task Management**: Create, read, update, and delete tasks with full ownership validation.
- **Task Status Management**: Update task status (PATCH endpoint for atomic status changes).
- **Ownership Validation**: Users can only access and modify their own tasks.
- **Entity Framework Core**: Database-agnostic ORM with SQLite default.
- **Domain-Driven Services**: Clean architecture with repository and service patterns.
- **Comprehensive Testing**: 99 unit tests across controllers, services, and business logic with ownership scenarios.
- **Production-Ready Database**: Entity Framework migrations for schema versioning.

## Technologies Used

### Backend
- **.NET 10.0**: Latest LTS framework version.
- **ASP.NET Core**: Web framework with built-in dependency injection and middleware.
- **Entity Framework Core**: ORM for database access with automatic migrations.
- **JWT (JSON Web Tokens)**: Bearer token authentication via `Microsoft.AspNetCore.Authentication.JwtBearer`.
- **xUnit**: Unit testing framework.
- **Moq**: Mocking library for unit tests.
- **SQLite**: Default lightweight database (configurable in `appsettings.json`).

### Frontend
- **React 18**: Modern UI library with hooks.
- **TypeScript**: Type-safe development.
- **Vite**: Fast build tool and dev server.
- **Tailwind CSS**: Utility-first CSS framework for styling.
- **React Router**: Client-side routing.
- **Zustand**: Lightweight state management.
- **Axios**: HTTP client for API integration.
- **Lucide React**: Modern icon library.

## Prerequisites

- [.NET SDK 10.0](https://dotnet.microsoft.com/download/dotnet/10.0) or higher.
- [Node.js 16+](https://nodejs.org/) and npm.
- A code editor, such as Visual Studio Code or Visual Studio.

## How to Run

### Option 1: Run Both (Backend + Frontend) Separately

#### Backend
1. **Clone the repository**:
   ```bash
   git clone https://github.com/pedroprata44/TaskManager.git
   cd TaskManager
   ```

2. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

3. **Run database migrations** (if necessary):
   ```bash
   cd TaskManager.Api
   dotnet ef database update
   ```

4. **Run the API**:
   ```bash
   dotnet run --project TaskManager.Api
   ```
   The API will be available at `https://localhost:5001`.

#### Frontend
1. **Navigate to frontend directory**:
   ```bash
   cd taskmanager-frontend
   ```

2. **Install dependencies**:
   ```bash
   npm install
   ```

3. **Create `.env` file**:
   ```bash
   cp .env.example .env
   ```

4. **Run the development server**:
   ```bash
   npm run dev
   ```
   The frontend will be available at `http://localhost:5173`.

### Option 2: Using Docker Compose (Recommended)

Make sure Docker is installed and running, then:

```bash
docker-compose up
```

This will start both the frontend (port 5173) and a development environment. You'll still need to run the backend manually on your machine since it requires .NET SDK.

## How to Test

### Backend Tests
To run the comprehensive unit test suite (99 tests):

```bash
dotnet test
```

To run with code coverage reporting:

```bash
dotnet test --collect:"XPlat Code Coverage" && reportgenerator -reports:"TaskManager.Tests/TestResults/*/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
```

**Test Coverage Includes:**
- Authentication controller and service tests
- Task CRUD operations across all HTTP verbs
- Multi-user task ownership validation
- Authorization checks (403 Forbidden for unauthorized access)
- Edge cases and error scenarios

Tests are located in the `TaskManager.Tests` project with dedicated test classes for each controller and service.

### Frontend Tests
To run the comprehensive frontend test suite (50+ tests):

```bash
cd taskmanager-frontend
npm test
```

To run with UI:
```bash
npm test:ui
```

To generate coverage report:
```bash
npm test:coverage
```

**Frontend Test Coverage Includes:**
- API client integration tests
- Zustand store tests
- Component tests (Login, Register, Dashboard, etc)
- Page-level integration tests
- End-to-end authentication and task management flows
- Mock HTTP requests with MSW

For detailed frontend testing guide, see [taskmanager-frontend/TESTING.md](taskmanager-frontend/TESTING.md)

## Using the Frontend

### Login & Registration
1. Navigate to `http://localhost:5173`
2. Register a new account with email, username, and password
3. Or login with existing credentials

### Dashboard Features
- **Task Statistics**: View counts of Pending, In Progress, and Completed tasks
- **Create Task**: Click "+ Nova Tarefa" button to create a new task
- **View Tasks**: All your tasks are displayed with their status
- **Update Status**: Click the status icon to cycle through Pending → In Progress → Completed
- **Edit Task**: Click the edit icon to modify task title and description
- **Delete Task**: Click the delete icon to remove a task
- **Logout**: Click the "Sair" button in the top-right to logout

## Project Structure

- **TaskManager.Api**: Main API project (Backend)
  - **Controllers**: HTTP endpoint handlers
    - `AuthController.cs`: Register and login endpoints
    - `TasksController.cs`: Task CRUD operations with ownership validation
  - **Models**: Data transfer objects and entities
    - `User.cs`: User domain model
    - `TaskItem.cs`: Task domain model with UserId foreign key
    - `LoginRequest.cs`, `RegisterRequest.cs`, `AuthResponse.cs`: DTO models
  - **Data**: Database layer
    - `TaskDbContext.cs`: Entity Framework DbContext
  - **Services**: Business logic layer
    - `AuthService.cs`: Registration, password hashing, claims building
    - `TaskService.cs`: Task CRUD with ownership validation
    - `IAuthService.cs`, `ITaskService.cs`: Service contracts
  - **Repositories**: Data access layer
    - `EfTaskRepository.cs`: Entity Framework implementation (used in production)
    - `InMemoryTaskRepository.cs`: In-memory implementation (used in testing)
    - `ITaskRepository.cs`: Repository contract
  - **Migrations**: Entity Framework Core database schema versions

- **TaskManager.Tests**: Unit testing project
  - `AuthControllerTests.cs`: Authentication endpoint tests
  - `AuthServiceTests.cs`: Registration and login logic tests
  - `TasksControllerTests.cs`: Task endpoint tests
  - `TasksControllerOwnershipTests.cs`: Authorization and ownership validation tests
  - `TaskServiceTests.cs`: Task business logic tests
  - `TaskOwnershipTests.cs`: Cross-user access restriction tests

- **taskmanager-frontend**: React frontend application
  - **src/components**: Reusable React components
    - `ProtectedRoute.tsx`: Route protection for authenticated users
    - `TaskForm.tsx`: Task creation form
    - `TaskList.tsx`: Task list display with status management
    - `TaskEditModal.tsx`: Task editing modal
  - **src/pages**: Page components
    - `Login.tsx`: Login page
    - `Register.tsx`: User registration page
    - `Dashboard.tsx`: Main task management dashboard
  - **src/services**: API client
    - `api.ts`: Axios-based API client with JWT interceptors
  - **src/store**: Zustand state management
    - `authStore.ts`: Authentication state (token, user)
    - `taskStore.ts`: Task state (list, loading, errors)
  - **src/types**: TypeScript definitions
    - `index.ts`: All shared types and interfaces
  - **src/App.tsx**: Main app component with routing
  - **src/main.tsx**: Entry point
  - **src/index.css**: Global styles with Tailwind

## Authentication & Authorization

### JWT Flow
1. User calls `POST /api/auth/register` or `POST /api/auth/login`
2. Server validates credentials and returns a JWT token
3. Client includes token in `Authorization: Bearer <token>` header for protected endpoints
4. Server validates token signature and claims before processing request

### Ownership Model
- Every `TaskItem` has a `UserId` field linking to the creating user
- Task operations (GET, PUT, DELETE) extract the user ID from JWT claims
- `TaskService` validates that the requester owns the task before allowing modifications
- Unauthorized access attempts return **403 Forbidden** (not 404, to prevent enumeration)

## API Endpoints

All task endpoints require JWT authentication. Obtain a token via the login endpoint first.

### Authentication Endpoints
- `POST /api/auth/register`: Register a new user with email, username, and password.
  ```json
  {
    "email": "user@example.com",
    "username": "username",
    "password": "securePassword123"
  }
  ```
- `POST /api/auth/login`: Authenticate and receive a JWT bearer token.
  ```json
  {
    "username": "username",
    "password": "securePassword123"
  }
  ```
  **Response:**
  ```json
  {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresIn": 3600
  }
  ```

### Task Endpoints (Require Authorization Header: `Authorization: Bearer <token>`)
- `GET /api/tasks`: Lists all tasks belonging to the authenticated user.
- `GET /api/tasks/{id}`: Gets a specific task by ID (403 Forbidden if not owner).
- `POST /api/tasks`: Creates a new task. Owner is automatically set to authenticated user.
  ```json
  {
    "title": "My Task",
    "description": "Task description",
    "status": "Pending"
  }
  ```
- `PUT /api/tasks/{id}`: Updates an existing task (403 Forbidden if not owner).
- `PATCH /api/tasks/{id}/status`: Atomically updates task status (Pending, InProgress, Completed).
  ```json
  {
    "status": "Completed"
  }
  ```
- `DELETE /api/tasks/{id}`: Deletes a task (403 Forbidden if not owner).

### Testing Endpoints

Use the `TaskManager.Api.http` file with VS Code's REST Client extension to test endpoints interactively:
```
# test.http
POST https://localhost:5001/api/auth/login
Content-Type: application/json

{
  "username": "testuser",
  "password": "Test@1234"
}
```

Copy the returned token and add it to subsequent requests in the `Authorization` header.

## Configuration

### JWT Settings (`appsettings.json`)
```json
{
  "Jwt": {
    "Key": "YourSuperSecretKeyHere12345678901234567890",
    "Issuer": "TaskManager",
    "Audience": "TaskManagerUsers"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=taskmanager.db"
  }
}
```

**Important**: Change the `Jwt:Key` value in production to a secure, randomly generated 32+ character string.

### Database Configuration
- Default: SQLite database (`taskmanager.db`)
- Supports Entity Framework Core connection strings for any EF Core provider (SQL Server, PostgreSQL, MySQL, etc.)
- Modify `ConnectionStrings:DefaultConnection` in `appsettings.json`

## Contributing

Contributions are welcome! Follow these steps:

1. Fork the project.
2. Create a branch for your feature (`git checkout -b feature/new-feature`).
3. Commit your changes (`git commit -am 'Add new feature'`).
4. Push to the branch (`git push origin feature/new-feature`).
5. Open a Pull Request.

## License

This project is licensed under the MIT License. See the LICENSE file for more details.
