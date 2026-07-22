using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShellMgmt.Application.AppService.Create;
using ShellMgmt.Application.AppService.Delete;
using ShellMgmt.Application.AppService.Get;
using ShellMgmt.Application.AppService.Update;

namespace ShellMgmt.Api.Controllers.Apps;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion(1)]
[Authorize]
public sealed class AppController(IMediator mediator, ILogger<AppController> logger) : ControllerBase
{
    [HttpGet("Get")]
    public async Task<IActionResult> Get([FromQuery] GetAppQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("GetById/{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var query = new GetAppQuery(Id: id, Take: 1);
        var result = await mediator.Send(query, cancellationToken);
        var app = result.Items.FirstOrDefault();
        if (app == null) return NotFound(new { error = "Application not found" });
        return Ok(app);
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] CreateAppCommand command, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(command.Name))
                return BadRequest(new { error = "Name is required" });
            if (string.IsNullOrWhiteSpace(command.Code))
                return BadRequest(new { error = "Code is required" });

            var result = await mediator.Send(command, cancellationToken);
            logger.LogInformation("Application created: {Name} ({Code})", command.Name, command.Code);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating application");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPut("Update")]
    public async Task<IActionResult> Update([FromBody] UpdateAppCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var result = await mediator.Send(command, cancellationToken);
            if (result == null) return NotFound(new { error = "Application not found" });
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating application {Id}", command.Id);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await mediator.Send(new DeleteAppCommand(id), cancellationToken);
            if (!result) return NotFound(new { error = "Application not found" });
            return Ok(new { message = "Application deleted successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting application {Id}", id);
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
