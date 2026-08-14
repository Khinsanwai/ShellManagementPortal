using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShellMgmt.Application.UserlogService.Create;
using ShellMgmt.Application.UserlogService.Get;

namespace ShellMgmt.Api.Controllers.Userlogs;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion(1)]
public sealed class UserlogController(IMediator mediator, ILogger<UserlogController> logger) : ControllerBase
{
    [HttpGet("Get")]
    [Authorize]
    public async Task<IActionResult> Get([FromQuery] GetUserlogQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("Dashboard")]
    [Authorize]
    public async Task<IActionResult> GetDashboard(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAuditDashboardQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost("Create")]
    [AllowAnonymous]
    public async Task<IActionResult> Create([FromBody] CreateUserlogCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var result = await mediator.Send(command, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating user log");
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
