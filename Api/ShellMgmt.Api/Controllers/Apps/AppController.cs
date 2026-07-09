using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShellMgmt.Application.AppService.Get;

namespace ShellMgmt.Api.Controllers.Apps;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion(1)]
[Authorize]
public sealed class AppController(IMediator mediator) : ControllerBase
{
    [HttpGet("Get")]
    public async Task<IActionResult> Get([FromQuery] GetAppQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
