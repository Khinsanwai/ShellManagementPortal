# Run Project

Build and run both the API and Web projects.

## Steps

### 1. Build Solution

```bash
dotnet build ShellManagementPortal.sln
```

Fix any build errors before proceeding.

### 2. Run API (in background)

```bash
dotnet run --project Api/ShellMgmt.Api/ShellMgmt.Api.csproj
```

API will start at:
- HTTPS: https://localhost:7203
- HTTP: http://localhost:5081

### 3. Run Web (in background)

```bash
dotnet run --project Web/ShellMgmt.Web.csproj
```

Web will start at:
- HTTPS: https://localhost:7067
- HTTP: http://localhost:5255

### 4. Prerequisites

- **SQL Server**: `KHINSANWAI\SQLEXPRESS` must be running with `SMPortal` database
- **WSO2 Identity Server**: Must be running at https://localhost:9443 for authentication
- Both must be running for the app to function fully

### 5. Quick Health Check

After both are running:
- Swagger UI: https://localhost:7203/swagger
- Web App: https://localhost:7067

## Notes

- In Visual Studio, use the `.slnLaunch.user` profile to start both projects simultaneously
- The Web project depends on the API — start API first
- If ports conflict, check `Properties/launchSettings.json` in each project
