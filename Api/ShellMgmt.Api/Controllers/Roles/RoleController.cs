using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShellMgmt.Api.Services;
using ShellMgmt.Domain.UserModels;

namespace ShellMgmt.Api.Controllers.Roles;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion(1)]
[Authorize]
public sealed class RoleController(ScimService scimService, ILogger<RoleController> logger) : ControllerBase
{
    [HttpGet("Get")]
    public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var roles = await scimService.GetRolesAsync(startIndex: (page - 1) * pageSize + 1, count: pageSize);
            return Ok(new { items = roles, total = roles.Count });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching roles");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("Get/{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        try
        {
            var role = await scimService.GetRoleAsync(id);
            if (role == null) return NotFound(new { error = "Role not found" });
            return Ok(role);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching role {Id}", id);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] CreateRoleRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.DisplayName))
                return BadRequest(new { error = "Display name is required" });

            if (request.DisplayName.Length < 3 || request.DisplayName.Length > 30)
                return BadRequest(new { error = "Role name must be between 3 and 30 characters" });

            if (request.DisplayName.Contains(' '))
                return BadRequest(new { error = "Role name cannot contain spaces" });

            var result = await scimService.CreateRoleAsync(request.DisplayName);
            logger.LogInformation("Role created: {Name} ({Id})", result.DisplayName, result.Id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating role");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPut("Update/{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] CreateRoleRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.DisplayName))
                return BadRequest(new { error = "Display name is required" });

            if (request.DisplayName.Length < 3 || request.DisplayName.Length > 30)
                return BadRequest(new { error = "Role name must be between 3 and 30 characters" });

            if (request.DisplayName.Contains(' '))
                return BadRequest(new { error = "Role name cannot contain spaces" });

            var existing = await scimService.GetRoleAsync(id);
            if (existing == null) return NotFound(new { error = "Role not found" });

            var result = await scimService.UpdateRoleAsync(id, request.DisplayName);
            logger.LogInformation("Role updated: {Name} ({Id})", result.DisplayName, result.Id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating role {Id}", id);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            var existing = await scimService.GetRoleAsync(id);
            if (existing == null) return NotFound(new { error = "Role not found" });

            var result = await scimService.DeleteRoleAsync(id);
            if (!result) return StatusCode(500, new { error = "Failed to delete role" });

            logger.LogInformation("Role deleted: {Name} ({Id})", existing.DisplayName, id);
            return Ok(new { message = "Role deleted successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting role {Id}", id);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("SyncRoleGroups/{roleId}")]
    public async Task<IActionResult> SyncRoleGroups(string roleId, [FromBody] SyncRoleGroupsRequest request)
    {
        try
        {
            var role = await scimService.GetRoleAsync(roleId);
            if (role == null) return NotFound(new { error = "Role not found" });

            await scimService.SyncRoleGroupsAsync(roleId, request.GroupIds);
            logger.LogInformation("Groups synced for role {Id} ({Name})", roleId, role.DisplayName);
            return Ok(new { message = "Role group assignments updated successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error syncing groups for role {Id}", roleId);
            return StatusCode(500, new { error = ex.Message });
        }
    }
}

public class CreateRoleRequest
{
    public string DisplayName { get; set; } = string.Empty;
}

public class SyncRolesRequest
{
    public List<string> RoleIds { get; set; } = new();
}

public class SyncRoleGroupsRequest
{
    public List<string> GroupIds { get; set; } = new();
}
