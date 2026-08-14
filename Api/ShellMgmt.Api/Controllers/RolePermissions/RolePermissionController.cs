using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShellMgmt.Api.Services;
using ShellMgmt.Domain.UserModels;

namespace ShellMgmt.Api.Controllers.RolePermissions;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion(1)]
[Authorize]
public sealed class RolePermissionController(ScimService scimService, ILogger<RolePermissionController> logger) : ControllerBase
{
    [HttpGet("GetByRole/{roleName}")]
    public async Task<IActionResult> GetByRole(string roleName)
    {
        try
        {
            var permissions = await scimService.GetRolePermissionsAsync(roleName);
            return Ok(permissions);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching permissions for role: {RoleName}", roleName);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("GetByUser/{userId}")]
    public async Task<IActionResult> GetByUser(string userId)
    {
        try
        {
            var permissions = await scimService.GetUserRolePermissionsAsync(userId);
            return Ok(permissions);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching permissions for user: {UserId}", userId);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("GetByCurrentUser")]
    public async Task<IActionResult> GetByCurrentUser()
    {
        try
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value
                         ?? User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { error = "User ID not found in claims" });
            }

            var permissions = await scimService.GetUserRolePermissionsAsync(userId);
            return Ok(permissions);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching permissions for current user");
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
