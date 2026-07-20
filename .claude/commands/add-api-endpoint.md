# Add API Endpoint

Add a custom API endpoint to an existing controller or create a new controller.

## Input

Describe the endpoint: `/add-api-endpoint POST /api/v1/product/bulk-delete`

## Steps

### Option A: Add to Existing Controller

1. Open the relevant controller in `Api/ShellMgmt.Api/Controllers/{Entities}/`
2. Add the new action method following existing patterns:
   - Use `[HttpGet]`, `[HttpPost]`, `[HttpPut]`, or `[HttpDelete]` attributes
   - Inject `IMediator` via primary constructor (already available)
   - Accept query params with `[FromQuery]` or body with `[FromBody]`
   - Always pass `CancellationToken cancellationToken`
   - Return `IActionResult` via `Ok()`, `NotFound()`, `BadRequest()`

### Option B: Create New Controller

Create `Api/ShellMgmt.Api/Controllers/{Entities}/{Entity}Controller.cs`:
```csharp
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShellMgmt.Api.Controllers.{Entities};

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion(1)]
[Authorize]
public sealed class {Entity}Controller(IMediator mediator) : ControllerBase
{
    // Add actions here
}
```

### CQRS Layer (if needed)

If the endpoint requires new business logic, create a command/query in `Api/ShellMgmt.Application/{Entity}Service/`:

**Command** (write operation):
```csharp
using MediatR;

namespace ShellMgmt.Application.{Entity}Service.{Operation};

public sealed record {Operation}{Entity}Command(...) : IRequest<{ReturnType}>;
```

**Handler**:
```csharp
using MediatR;
using SharedKernel.Domain;

namespace ShellMgmt.Application.{Entity}Service.{Operation};

internal sealed class {Operation}{Entity}CommandHandler(IRepository<{Entity}> repository, IUnitOfWork unitOfWork)
    : IRequestHandler<{Operation}{Entity}Command, {ReturnType}>
{
    public async Task<{ReturnType}> Handle({Operation}{Entity}Command command, CancellationToken cancellationToken)
    {
        // Implementation
    }
}
```

**Query** (read operation):
```csharp
using MediatR;
using SharedKernel.Domain;

namespace ShellMgmt.Application.{Entity}Service.{Operation};

public sealed record {Operation}{Entity}Query(...) : IRequest<PagedList<{Entity}Dto>>;
```

### Convention Notes

- Route pattern: `api/v{version:apiVersion}/[controller]/{action}`
- Controller names are pluralized (e.g., `Products`, `Claims`)
- All endpoints require `[Authorize]` by default
- Use `[AllowAnonymous]` only for public endpoints
- Always accept `CancellationToken cancellationToken` as last parameter
