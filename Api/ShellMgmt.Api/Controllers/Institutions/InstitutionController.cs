using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShellMgmt.Application.InstitutionService.Create;
using ShellMgmt.Application.InstitutionService.Delete;
using ShellMgmt.Application.InstitutionService.Get;
using ShellMgmt.Application.InstitutionService.Update;

namespace ShellMgmt.Api.Controllers.Institutions;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion(1)]
[Authorize]
public sealed class InstitutionController(IMediator mediator) : ControllerBase
{
    [HttpGet("Get")]
    public async Task<IActionResult> Get([FromQuery] GetInstitutionQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] CreateInstitutionCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPut("Update")]
    public async Task<IActionResult> Update([FromBody] UpdateInstitutionCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteInstitutionCommand(id), cancellationToken);
        if (!result) return NotFound();
        return Ok();
    }
}
