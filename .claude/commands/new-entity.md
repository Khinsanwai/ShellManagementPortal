# New Entity Scaffold

Scaffold a complete new entity across all layers of the CQRS architecture.

## Input

The user will provide: **Entity name** and **properties** (name + type + required/optional).

Example: `/new-entity Product (Name:string:required, Price:decimal:required, Description:string:optional)`

## Steps

### 1. Domain Layer — `Api/ShellMgmt.Domain/{Entity}Models/`

Create two files:

**{Entity}.cs** — Entity class:
```csharp
namespace ShellMgmt.Domain.{Entity}Models;

public class {Entity}
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    // ... other properties: required string for mandatory, string? for optional
}
```

**{Entity}Dto.cs** — DTO class (all properties nullable):
```csharp
namespace ShellMgmt.Domain.{Entity}Models;

public class {Entity}Dto
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    // ... mirror entity but all nullable
}
```

### 2. Application Layer — `Api/ShellMgmt.Application/{Entity}Service/`

Create subfolders and files:

**Create/Create{Entity}Command.cs**:
```csharp
using MediatR;
using ShellMgmt.Domain.{Entity}Models;

namespace ShellMgmt.Application.{Entity}Service.Create;

public sealed record Create{Entity}Command(string Name, ...) : IRequest<{Entity}Dto>;
```

**Create/Create{Entity}CommandHandler.cs**:
```csharp
using AutoMapper;
using MediatR;
using SharedKernel.Domain;
using ShellMgmt.Domain.{Entity}Models;

namespace ShellMgmt.Application.{Entity}Service.Create;

internal sealed class Create{Entity}CommandHandler(IRepository<{Entity}> repository, IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<Create{Entity}Command, {Entity}Dto>
{
    public async Task<{Entity}Dto> Handle(Create{Entity}Command command, CancellationToken cancellationToken)
    {
        var entity = new {Entity}
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            // ... map other properties
        };

        await repository.Add(entity);
        await unitOfWork.SaveChanges(cancellationToken);

        return mapper.Map<{Entity}Dto>(entity);
    }
}
```

**Update/Update{Entity}Command.cs**:
```csharp
using MediatR;
using ShellMgmt.Domain.{Entity}Models;

namespace ShellMgmt.Application.{Entity}Service.Update;

public sealed record Update{Entity}Command(Guid Id, string Name, ...) : IRequest<{Entity}Dto?>;
```

**Update/Update{Entity}CommandHandler.cs**:
```csharp
using AutoMapper;
using MediatR;
using SharedKernel.Domain;
using ShellMgmt.Domain.{Entity}Models;

namespace ShellMgmt.Application.{Entity}Service.Update;

internal sealed class Update{Entity}CommandHandler(IRepository<{Entity}> repository, IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<Update{Entity}Command, {Entity}Dto?>
{
    public async Task<{Entity}Dto?> Handle(Update{Entity}Command command, CancellationToken cancellationToken)
    {
        var entity = await repository.GetById(command.Id);
        if (entity == null) return null;

        entity.Name = command.Name;
        // ... map other properties

        await repository.Update(entity);
        await unitOfWork.SaveChanges(cancellationToken);

        return mapper.Map<{Entity}Dto>(entity);
    }
}
```

**Delete/Delete{Entity}Command.cs**:
```csharp
using MediatR;

namespace ShellMgmt.Application.{Entity}Service.Delete;

public sealed record Delete{Entity}Command(Guid Id) : IRequest<bool>;
```

**Delete/Delete{Entity}CommandHandler.cs**:
```csharp
using MediatR;
using SharedKernel.Domain;
using ShellMgmt.Domain.{Entity}Models;

namespace ShellMgmt.Application.{Entity}Service.Delete;

internal sealed class Delete{Entity}CommandHandler(IRepository<{Entity}> repository, IUnitOfWork unitOfWork)
    : IRequestHandler<Delete{Entity}Command, bool>
{
    public async Task<bool> Handle(Delete{Entity}Command command, CancellationToken cancellationToken)
    {
        var entity = await repository.GetById(command.Id);
        if (entity == null) return false;

        await repository.Delete(entity);
        await unitOfWork.SaveChanges(cancellationToken);

        return true;
    }
}
```

**Get/Get{Entity}Query.cs**:
```csharp
using MediatR;
using SharedKernel.Domain;
using ShellMgmt.Domain.{Entity}Models;

namespace ShellMgmt.Application.{Entity}Service.Get;

public sealed record Get{Entity}Query(
    int Take = 50,
    int Skip = 1,
    string? SortBy = null,
    string? OrderBy = null,
    Guid? Id = null,
    string? Name = null) : IRequest<PagedList<{Entity}Dto>>;
```

**Get/Get{Entity}QueryHandler.cs**:
```csharp
using AutoMapper;
using MediatR;
using SharedKernel.Domain;
using ShellMgmt.Application.{Entity}Service.{Entity}Specification;
using ShellMgmt.Domain.{Entity}Models;

namespace ShellMgmt.Application.{Entity}Service.Get;

internal sealed class Get{Entity}QueryHandler(IReadRepository<{Entity}> repository, IMapper mapper)
    : IRequestHandler<Get{Entity}Query, PagedList<{Entity}Dto>>
{
    public async Task<PagedList<{Entity}Dto>> Handle(Get{Entity}Query query, CancellationToken cancellationToken)
    {
        {Entity}Dto dto = mapper.Map<{Entity}Dto>(query);
        {Entity}GetSpec spec = new(dto, query.Take, query.Skip, query.SortBy, query.OrderBy);
        var entities = await repository.Get(spec, cancellationToken);
        return mapper.Map<PagedList<{Entity}Dto>>(entities);
    }
}
```

**{Entity}Specification/{Entity}GetSpec.cs**:
```csharp
using SharedKernel.Persistance.Abstractions;
using System.Linq.Expressions;
using ShellMgmt.Domain.{Entity}Models;

namespace ShellMgmt.Application.{Entity}Service.{Entity}Specification;

public class {Entity}GetSpec : Specification<{Entity}>
{
    public {Entity}GetSpec({Entity}Dto dto, int take, int skip, string? sortBy, string? orderBy)
    {
        if (!string.IsNullOrEmpty(dto.Name))
        {
            ApplyCriteria(x => x.Name.Contains(dto.Name));
        }

        ApplyPaging(skip, take);

        var orderByExpression = GetSortProperty(sortBy);

        if (orderBy != null && orderBy.Equals("ASC", StringComparison.CurrentCultureIgnoreCase))
        {
            ApplyOrderBy(orderByExpression);
        }
        else
        {
            ApplyOrderByDescending(orderByExpression);
        }
    }

    private static Expression<Func<{Entity}, object>> GetSortProperty(string? sortBy)
    {
        return sortBy?.ToLower() switch
        {
            "id" => x => x.Id,
            "name" => x => x.Name,
            _ => x => x.Id
        };
    }

    public override Expression<Func<{Entity}, bool>> ToExpression()
    {
        throw new NotImplementedException();
    }
}
```

### 3. Persistence Layer — Update DbContext

In `Api/ShellMgmt.Persistence/ApplicationDbContext/AppDbContext.cs`:
- Add `public DbSet<{Entity}> {Entity} { get; set; }`
- Add in `OnModelCreating`: `modelBuilder.Entity<{Entity}>().ToTable("{Entity}", "appshell");`

Also update `ReadDbContext.cs` with the same DbSet.

### 4. API Controller — `Api/ShellMgmt.Api/Controllers/{Entities}/`

Create `{Entity}Controller.cs`:
```csharp
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShellMgmt.Application.{Entity}Service.Create;
using ShellMgmt.Application.{Entity}Service.Delete;
using ShellMgmt.Application.{Entity}Service.Get;
using ShellMgmt.Application.{Entity}Service.Update;

namespace ShellMgmt.Api.Controllers.{Entities};

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion(1)]
[Authorize]
public sealed class {Entity}Controller(IMediator mediator) : ControllerBase
{
    [HttpGet("Get")]
    public async Task<IActionResult> Get([FromQuery] Get{Entity}Query query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] Create{Entity}Command command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPut("Update")]
    public async Task<IActionResult> Update([FromBody] Update{Entity}Command command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new Delete{Entity}Command(id), cancellationToken);
        if (!result) return NotFound();
        return Ok();
    }
}
```

### 5. AutoMapper — Update `Api/ShellMgmt.Api/Mappers/AutoMapping.cs`

Add these mappings in the constructor:
```csharp
CreateMap<{Entity}, {Entity}Dto>();
CreateMap<PagedList<{Entity}>, PagedList<{Entity}Dto>>().ReverseMap();
CreateMap<Get{Entity}Query, {Entity}Dto>().ReverseMap();
CreateMap<Create{Entity}Command, {Entity}Dto>().ReverseMap();
```

### 6. Blazor Pages — `Web/Components/Pages/{Entity}/`

**{Entity}List.razor**:
```razor
@page "/{entities}"
@attribute [Authorize]
@inject ApiService ApiService
@inject NotificationService NotificationService
@inject DialogService DialogService
@inject NavigationManager NavigationManager

<PageTitle>{Entities} - Shell Management Portal</PageTitle>

<RadzenRow class="rz-p-4">
    <RadzenColumn Size="12">
        <RadzenStack Orientation="Orientation.Horizontal" JustifyContent="JustifyContent.SpaceBetween" AlignItems="AlignItems.Center">
            <RadzenStack Orientation="Orientation.Horizontal" AlignItems="AlignItems.Center" Gap="1rem">
                <RadzenButton Icon="arrow_back" ButtonStyle="ButtonStyle.Light" Click="@(() => NavigationManager.NavigateTo("/"))" Tooltip="Return to Dashboard" />
                <RadzenText TextStyle="TextStyle.H4">{Entities}</RadzenText>
            </RadzenStack>
            <RadzenButton Text="Add {Entity}" Icon="add" Click="@Create{Entity}" />
        </RadzenStack>
    </RadzenColumn>
</RadzenRow>

<RadzenDataGrid @ref="grid" Data="@items" TItem="{Entity}Dto" AllowSorting="true" AllowPaging="true" PageSize="10">
    <Columns>
        <RadzenDataGridColumn TItem="{Entity}Dto" Property="Name" Title="Name" />
        <!-- Add other columns here -->
        <RadzenDataGridColumn TItem="{Entity}Dto" Title="Actions" Width="200px">
            <Template Context="item">
                <RadzenButton Icon="edit" Size="ButtonSize.Small" Click="@(() => Edit{Entity}(item))" />
                <RadzenButton Icon="delete" Size="ButtonSize.Small" ButtonStyle="ButtonStyle.Danger" Click="@(() => Delete{Entity}(item))" />
            </Template>
        </RadzenDataGridColumn>
    </Columns>
</RadzenDataGrid>

@code {
    RadzenDataGrid<{Entity}Dto>? grid;
    IEnumerable<{Entity}Dto>? items;

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        var result = await ApiService.GetAsync<PagedList<{Entity}Dto>>("{entity}/get", AppConfig.AccessToken);
        items = result?.Items;
        if (grid != null) await grid.Reload();
    }

    private async Task Create{Entity}()
    {
        var result = await DialogService.OpenAsync<{Entity}Form>("Add {Entity}");
        if (result == true) await LoadData();
    }

    private async Task Edit{Entity}({Entity}Dto item)
    {
        var result = await DialogService.OpenAsync<{Entity}Form>("Edit {Entity}", new() { { "{Entity}Id", item.Id } });
        if (result == true) await LoadData();
    }

    private async Task Delete{Entity}({Entity}Dto item)
    {
        var confirmed = await DialogService.Confirm("Are you sure you want to delete this {entity}?", "Delete {Entity}");
        if (confirmed == true)
        {
            await ApiService.DeleteAsync("{entity}/delete/{item.Id}", AppConfig.AccessToken);
            NotificationService.Notify(NotificationSeverity.Success, "Deleted", "{Entity} deleted successfully");
            await LoadData();
        }
    }
}
```

**{Entity}Form.razor**:
```razor
@inject ApiService ApiService
@inject NotificationService NotificationService
@inject DialogService DialogService

<RadzenTemplateForm Data="@model" TItem="{Entity}Dto" Submit="@OnSubmit">
    <RadzenStack Gap="1rem">
        <RadzenFormField Text="Name">
            <RadzenTextBox @bind-Value="model.Name" Name="Name" />
            <RadzenRequiredValidator Component="Name" Text="Name is required" />
        </RadzenFormField>
        <!-- Add other form fields here -->
        <RadzenStack Orientation="Orientation.Horizontal" JustifyContent="JustifyContent.End">
            <RadzenButton Text="Cancel" ButtonStyle="ButtonStyle.Light" Click="@(() => DialogService.Close(false))" />
            <RadzenButton Text="Save" ButtonType="ButtonType.Submit" />
        </RadzenStack>
    </RadzenStack>
</RadzenTemplateForm>

@code {
    [Parameter] public Guid? {Entity}Id { get; set; }

    private {Entity}Dto model = new();

    protected override async Task OnInitializedAsync()
    {
        if ({Entity}Id.HasValue)
        {
            var result = await ApiService.GetAsync<PagedList<{Entity}Dto>>($"{{entity}}/get?Id={{{Entity}Id}}", AppConfig.AccessToken);
            model = result?.Items?.FirstOrDefault() ?? new {Entity}Dto();
        }
    }

    private async Task OnSubmit({Entity}Dto item)
    {
        if ({Entity}Id.HasValue)
        {
            await ApiService.PutAsync("{entity}/update", new { Id = {Entity}Id, item.Name }, AppConfig.AccessToken);
            NotificationService.Notify(NotificationSeverity.Success, "Updated", "{Entity} updated successfully");
        }
        else
        {
            await ApiService.PostAsync("{entity}/create", item, AppConfig.AccessToken);
            NotificationService.Notify(NotificationSeverity.Success, "Created", "{Entity} created successfully");
        }
        DialogService.Close(true);
    }
}
```

### 7. Update _Imports.razor

Add `@using ShellMgmt.Domain.{Entity}Models` to `Web/Components/_Imports.razor`.

### 8. Create EF Migration

```bash
dotnet ef migrations add Add{Entity} --project Api/ShellMgmt.Persistence
dotnet ef database update --project Api/ShellMgmt.Persistence
```

## Validation Checklist

After scaffolding, verify:
- [ ] Domain entity and DTO created
- [ ] All 4 CQRS operations (Create, Get, Update, Delete) with handlers
- [ ] Specification class created
- [ ] DbContext updated (both AppDbContext and ReadDbContext)
- [ ] API controller created with all 4 endpoints
- [ ] AutoMapper profile updated
- [ ] Blazor List and Form pages created
- [ ] _Imports.razor updated
- [ ] Migration created and applied
- [ ] Solution builds: `dotnet build ShellManagementPortal.sln`
