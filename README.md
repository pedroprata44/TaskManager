# TaskManager

A small .NET minimal API for managing tasks with Entity Framework Core and SQLite.

## Project Structure

- `TaskManager.Api/` — minimal API project
- `TaskManager.Tests/` — xUnit test project

## Features

- CRUD operations for task entities
- Audit timestamps on task models (`CreatedAt`, `UpdatedAt`)
- Swagger/OpenAPI support for browser testing
- In-memory and SQLite-backed persistence for API and tests

## Running the API

```bash
cd /workspaces/TaskManager/TaskManager.Api
dotnet run
```

Then open the Swagger UI in your browser, typically at `https://localhost:5001/swagger`.

## Running tests

```bash
cd /workspaces/TaskManager
dotnet test TaskManager.Tests/TaskManager.Tests.csproj
```

## Notes

- Tests should use randomized domain data rather than hardcoded values.
- Missing resources should result in exceptions like `KeyNotFoundException`, not `null`.
- The API is intentionally minimal; only task CRUD endpoints should be exposed unless additional endpoints are requested.
