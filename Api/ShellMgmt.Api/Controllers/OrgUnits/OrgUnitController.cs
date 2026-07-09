using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShellMgmt.Application.OrgUnitService.Create;
using ShellMgmt.Application.OrgUnitService.Delete;
using ShellMgmt.Application.OrgUnitService.Get;
using ShellMgmt.Application.OrgUnitService.Update;

namespace ShellMgmt.Api.Controllers.OrgUnits;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion(1)]
[Authorize]
public sealed class OrgUnitController(IMediator mediator) : ControllerBase
{
    [HttpGet("Get")]
    public async Task<IActionResult> Get([FromQuery] GetOrgUnitQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] CreateOrgUnitCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPut("Update")]
    public async Task<IActionResult> Update([FromBody] UpdateOrgUnitCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteOrgUnitCommand(id), cancellationToken);
        if (!result) return NotFound();
        return Ok();
    }
}
