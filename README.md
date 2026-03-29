# TaskManager

## Description

A modern RESTful API for task management built with ASP.NET Core 10.0. Features JWT authentication, multi-user support with task ownership, and comprehensive test coverage (99 passing tests).

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

- **.NET 10.0**: Latest LTS framework version.
- **ASP.NET Core**: Web framework with built-in dependency injection and middleware.
- **Entity Framework Core**: ORM for database access with automatic migrations.
- **JWT (JSON Web Tokens)**: Bearer token authentication via `Microsoft.AspNetCore.Authentication.JwtBearer`.
- **xUnit**: Unit testing framework.
- **Moq**: Mocking library for unit tests.
- **SQLite**: Default lightweight database (configurable in `appsettings.json`).

## Prerequisites

- [.NET SDK 10.0](https://dotnet.microsoft.com/download/dotnet/10.0) or higher.
- A code editor, such as Visual Studio Code or Visual Studio.

## How to Run

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

4. **Run the application**:
   ```bash
   dotnet run --project TaskManager.Api
   ```

   The API will be available at `https://localhost:5001` or as configured in `launchSettings.json`.

## How to Test

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

## Project Structure

- **TaskManager.Api**: Main API project.
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

## Deployment Checklist

- [ ] Update JWT Key for production (use Azure Key Vault, AWS Secrets Manager, or similar)
- [ ] Enable CORS for frontend origin
- [ ] Add input validation (FluentValidation recommended)
- [ ] Implement rate limiting
- [ ] Add request logging (Serilog recommended)
- [ ] Configure production database (SQL Server or PostgreSQL recommended)
- [ ] Enable HTTPS only
- [ ] Implement health check endpoints
- [ ] Add OpenAPI/Swagger documentation

## Contributing

Contributions are welcome! Follow these steps:

1. Fork the project.
2. Create a branch for your feature (`git checkout -b feature/new-feature`).
3. Commit your changes (`git commit -am 'Add new feature'`).
4. Push to the branch (`git push origin feature/new-feature`).
5. Open a Pull Request.

## License

This project is licensed under the MIT License. See the LICENSE file for more details.