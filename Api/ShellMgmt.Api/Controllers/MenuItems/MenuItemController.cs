using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShellMgmt.Application.MenuItemService.Get;

namespace ShellMgmt.Api.Controllers.MenuItems;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion(1)]
[Authorize]
public sealed class MenuItemController(IMediator mediator) : ControllerBase
{
    [HttpGet("Get")]
    public async Task<IActionResult> Get([FromQuery] GetMenuItemQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
