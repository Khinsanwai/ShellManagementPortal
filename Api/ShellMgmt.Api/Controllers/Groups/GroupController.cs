using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShellMgmt.Api.Services;
using ShellMgmt.Domain.UserModels;

namespace ShellMgmt.Api.Controllers.Groups;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion(1)]
[Authorize]
public sealed class GroupController(ScimService scimService, ILogger<GroupController> logger) : ControllerBase
{
    [HttpGet("Get")]
    public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var groups = await scimService.GetGroupsAsync(startIndex: (page - 1) * pageSize + 1, count: pageSize);
            return Ok(new { items = groups, total = groups.Count });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching groups");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("Get/{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        try
        {
            var group = await scimService.GetGroupAsync(id);
            if (group == null) return NotFound(new { error = "Group not found" });
            return Ok(group);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching group {Id}", id);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] CreateGroupRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.DisplayName))
                return BadRequest(new { error = "Display name is required" });

            if (request.DisplayName.Length < 3 || request.DisplayName.Length > 30)
                return BadRequest(new { error = "Group name must be between 3 and 30 characters" });

            if (request.DisplayName.Contains(' '))
                return BadRequest(new { error = "Group name cannot contain spaces" });

            var result = await scimService.CreateGroupAsync(request.DisplayName);
            logger.LogInformation("Group created: {Name} ({Id})", result.DisplayName, result.Id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating group");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPut("Update/{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] CreateGroupRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.DisplayName))
                return BadRequest(new { error = "Display name is required" });

            if (request.DisplayName.Length < 3 || request.DisplayName.Length > 30)
                return BadRequest(new { error = "Group name must be between 3 and 30 characters" });

            if (request.DisplayName.Contains(' '))
                return BadRequest(new { error = "Group name cannot contain spaces" });

            var existing = await scimService.GetGroupAsync(id);
            if (existing == null) return NotFound(new { error = "Group not found" });

            var result = await scimService.UpdateGroupAsync(id, request.DisplayName);
            logger.LogInformation("Group updated: {Name} ({Id})", result.DisplayName, result.Id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating group {Id}", id);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            var existing = await scimService.GetGroupAsync(id);
            if (existing == null) return NotFound(new { error = "Group not found" });

            var result = await scimService.DeleteGroupAsync(id);
            if (!result) return StatusCode(500, new { error = "Failed to delete group" });

            logger.LogInformation("Group deleted: {Name} ({Id})", existing.DisplayName, id);
            return Ok(new { message = "Group deleted successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting group {Id}", id);
            return StatusCode(500, new { error = ex.Message });
        }
    }
}

public class CreateGroupRequest
{
    public string DisplayName { get; set; } = string.Empty;
}
