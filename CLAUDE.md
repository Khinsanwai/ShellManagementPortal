# CLAUDE.md — Shell Management Portal

## Project Overview

Shell Management Portal is a full-stack enterprise admin portal for managing identity, authorization, and application configuration. It consists of:

- **ShellMgmt.Api** — ASP.NET Core Web API backend (CQRS with MediatR)
- **ShellMgmt.Web** — Blazor Server frontend (Radzen UI components)

Integrates with **WSO2 Identity Server** for authentication (OIDC) and user/group management (SCIM 2.0).

## Tech Stack

- C# / .NET 8.0
- Entity Framework Core 8 (SQL Server, Code-First)
- MediatR (CQRS), AutoMapper, FluentValidation
- Blazor Server + Radzen.Blazor
- WSO2 Identity Server (OIDC + SCIM 2.0)
- Serilog (logging)

## Build & Run

```bash
# Build entire solution
dotnet build ShellManagementPortal.sln

# Restore packages
dotnet restore ShellManagementPortal.sln

# Run API (https://localhost:7203)
dotnet run --project Api/ShellMgmt.Api/ShellMgmt.Api.csproj

# Run Web (https://localhost:7067)
dotnet run --project Web/ShellMgmt.Web.csproj

# Apply EF Core migrations
dotnet ef database update --project Api/ShellMgmt.Persistence
```

Both API and Web must be running for the app to function. Use the `.slnLaunch.user` profile in Visual Studio to start both simultaneously.

## Project Structure

```
ShellManagementPortal/
├── SharedKernel/                        # Pre-compiled shared DLLs (not editable)
│   ├── SharedKernel.Domain.dll          # IRepository, IUnitOfWork, PagedList, ISpecification
│   ├── SharedKernel.Persistance.dll     # SpecificationEvaluator
│   └── SharedKernel.Application.dll
├── database/                            # SQL seed scripts
├── Api/
│   ├── ShellMgmt.Domain/               # Entities + DTOs
│   │   ├── AppModels/
│   │   ├── ClaimModels/
│   │   ├── InstitutionModels/
│   │   ├── MenuItemModels/              # MenuItem, UserMenuAssignment
│   │   ├── OrgUnitModels/
│   │   ├── ResourceModels/
│   │   ├── TenantModels/
│   │   └── UserModels/                  # ScimModels, ScimGroupModels
│   ├── ShellMgmt.Application/           # CQRS commands/queries + specs
│   │   ├── AppService/
│   │   ├── ClaimService/
│   │   ├── InstitutionService/
│   │   ├── MenuItemService/
│   │   ├── OrgUnitService/
│   │   ├── ResourceService/
│   │   └── TenantService/
│   ├── ShellMgmt.Persistence/           # EF Core DbContexts, repositories, migrations
│   │   ├── ApplicationDbContext/
│   │   ├── Ef.Repository/
│   │   └── Migrations/
│   └── ShellMgmt.Api/                   # API host (controllers, services, config)
│       ├── Controllers/
│       ├── Services/ScimService.cs      # WSO2 SCIM 2.0 client
│       └── Mappers/AutoMapping.cs
└── Web/
    ├── Components/
    │   ├── Layout/MainLayout.razor      # Sidebar + header layout
    │   └── Pages/                       # Feature pages (Claim, Group, User, etc.)
    ├── Controllers/AccountController.cs # OIDC login/logout
    ├── Services/                        # ApiService, MenuService, Wso2Service
    └── Constants/AppConfig.cs           # Static config (URLs, tenant info)
```

## Architecture Patterns

- **CQRS**: MediatR separates commands (write) and queries (read). Each entity has Create/Update/Delete/Get handlers with specifications.
- **Repository + Unit of Work**: Generic repos from SharedKernel. Separate `AppDbContext` (write) and `ReadDbContext` (read).
- **Dual Identity**: Users/groups managed externally via WSO2 SCIM 2.0. Menu items, claims, institutions stored locally in SQL Server.
- **Menu Authorization**: `UserMenuAssignment` maps WSO2 user IDs to local menu items to control sidebar visibility.

## Port Configuration

| Service | HTTPS | HTTP |
|---------|-------|------|
| API     | https://localhost:7203 | http://localhost:5081 |
| Web     | https://localhost:7067 | http://localhost:5255 |

## Database

- **Server**: SQL Server (local `KHINSANWAI\SQLEXPRESS`)
- **Database**: `SMPortal`
- **Schemas**: `appshell` (menus, claims, institutions, org units, apps), `keycloak` (tenants, resources)
- **Migrations**: Run from `Api/ShellMgmt.Persistence`

## Code Conventions

- Follow existing CQRS structure: new entities get Domain models, Application commands/queries/specs, Persistence config, and API controllers.
- Use FluentValidation for request validation in the Application layer.
- Use AutoMapper profiles in `Api/ShellMgmt.Api/Mappers/AutoMapping.cs`.
- Blazor pages use Radzen components — match existing page patterns (List + Form pages).
- API controllers use `api/v1/` route prefix with `[ApiVersion("1.0")]`.
- SharedKernel DLLs define interfaces (`IRepository`, `IUnitOfWork`, `ISpecification`) — do not try to modify them; implement against them.

## Warnings

- **No tests exist.** There are no test projects or test frameworks configured.
- **No CI/CD.** The `.github/workflows/` directory is empty.
- **No Docker.** No containerization is set up.
- **Credentials in config.** `appsettings.json` files contain hardcoded WSO2 secrets — do not commit real credentials.
- **Debug panel visible.** `MainLayout.razor` has a development debug alert showing auth state — remove before production.
