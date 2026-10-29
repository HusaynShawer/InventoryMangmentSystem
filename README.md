# InventoryMangmentSystem
ASP.NET Core Inventory Management System built with C#, Entity Framework Core, and SQL Server for practicing backend development and database management.

## Architecture

The backend follows a pragmatic Clean Architecture split with dependencies pointing inward:

```
src/
  InventoryManagementSystem.Domain/          # Entities, enums, domain exceptions
  InventoryManagementSystem.Application/     # DTOs, services, repository/identity abstractions
  InventoryManagementSystem.Infrastructure/  # EF Core DbContext, repositories, migrations, JWT/BCrypt
  InventoryManagementSystem.Api/             # Controllers, middleware, Program.cs, configuration
frontend/                                    # React + Vite client
```

Rules enforced by the project references:

- `Domain` has no dependencies.
- `Application` depends on `Domain` only.
- `Infrastructure` depends on `Application` (and `Domain`).
- `Api` depends on `Application` and `Infrastructure`.

Build and run:

```bash
dotnet build InventoryManagementSystem.slnx
dotnet run --project src/InventoryManagementSystem.Api
```

EF Core migrations target the Infrastructure project with the Api project as startup:

```bash
dotnet ef migrations add <Name> \
  --project src/InventoryManagementSystem.Infrastructure \
  --startup-project src/InventoryManagementSystem.Api
```
