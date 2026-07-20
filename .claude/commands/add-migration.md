# Add EF Core Migration

Create and apply a new Entity Framework Core migration.

## Input

Migration name: `/add-migration AddProductTable`

## Steps

### 1. Ensure DbContext is Updated

Verify that `AppDbContext.cs` and `ReadDbContext.cs` in `Api/ShellMgmt.Persistence/ApplicationDbContext/` have the new DbSet and table mapping:

```csharp
public DbSet<NewEntity> NewEntity { get; set; }

// In OnModelCreating:
modelBuilder.Entity<NewEntity>().ToTable("NewEntity", "appshell");
```

### 2. Create Migration

```bash
dotnet ef migrations add {MigrationName} --project Api/ShellMgmt.Persistence
```

### 3. Review Generated Migration

Check the generated file in `Api/ShellMgmt.Persistence/Migrations/` to verify:
- Table name and schema are correct
- Column types match entity properties
- Nullable/required constraints are correct
- Indexes are added if needed

### 4. Apply Migration

```bash
dotnet ef database update --project Api/ShellMgmt.Persistence
```

### 5. Verify

```bash
dotnet build ShellManagementPortal.sln
```

## Common Issues

- **Connection string**: Ensure `KHINSANWAI\SQLEXPRESS` SQL Server instance is running
- **Multiple DbContexts**: The `--project` flag targets the Persistence project. If both DbContexts need changes, ensure both are updated before running migration
- **Schema**: Local entities use `appshell`, tenant/resource entities use `keycloak`

## Rollback (if needed)

```bash
dotnet ef database update {PreviousMigrationName} --project Api/ShellMgmt.Persistence
dotnet ef migrations remove --project Api/ShellMgmt.Persistence
```
