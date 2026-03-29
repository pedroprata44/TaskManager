# TaskManager

## Description

The TaskManager is a RESTful API for task management, built with ASP.NET Core. It allows users to create, list, update, and delete tasks, and includes basic authentication features.

## Features

- **Task Management**: Create, read, update, and delete tasks.
- **Authentication**: Basic login system for users.
- **Database**: Uses Entity Framework Core with SQLite (or other configured database).
- **Unit Tests**: Test coverage for main services.

## Technologies Used

- **.NET 10.0**: Main framework.
- **ASP.NET Core**: For building the API.
- **Entity Framework Core**: ORM for database access.
- **xUnit**: Testing framework.
- **SQLite**: Default database (configurable via appsettings.json).

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

To run unit tests:

```bash
dotnet test
```

Tests are located in the `TaskManager.Tests` project.

## Project Structure

- **TaskManager.Api**: Main API project.
  - **Controllers**: Controllers for endpoints (AuthController, TasksController).
  - **Models**: Data models (TaskItem, LoginRequest).
  - **Data**: Database context (TaskDbContext).
  - **Repositories**: Repository implementations (EfTaskRepository, InMemoryTaskRepository).
  - **Services**: Business services (TaskService).
  - **Migrations**: Entity Framework migrations.
- **TaskManager.Tests**: Unit testing project.

## API Endpoints

### Authentication
- `POST /api/auth/login`: Performs login and returns a token (if implemented).

### Tasks
- `GET /api/tasks`: Lists all tasks.
- `GET /api/tasks/{id}`: Gets a task by ID.
- `POST /api/tasks`: Creates a new task.
- `PUT /api/tasks/{id}`: Updates an existing task.
- `DELETE /api/tasks/{id}`: Deletes a task.

To test the endpoints, you can use the `TaskManager.Api.http` file with VS Code's REST Client.

## Contributing

Contributions are welcome! Follow these steps:

1. Fork the project.
2. Create a branch for your feature (`git checkout -b feature/new-feature`).
3. Commit your changes (`git commit -am 'Add new feature'`).
4. Push to the branch (`git push origin feature/new-feature`).
5. Open a Pull Request.

## License

This project is licensed under the MIT License. See the LICENSE file for more details.