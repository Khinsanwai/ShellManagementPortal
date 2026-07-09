using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShellMgmt.Application.TenantService.GetByName;

namespace ShellMgmt.Api.Controllers.Tenants;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion(1)]
[Authorize]
public sealed class TenantController(IMediator mediator) : ControllerBase
{
    [HttpGet("GetByName/{name}")]
    public async Task<IActionResult> GetByName(string name, CancellationToken cancellationToken)
    {
        var query = new GetTenantByNameQuery(name);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
