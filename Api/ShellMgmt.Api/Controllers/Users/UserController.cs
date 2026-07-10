using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShellMgmt.Api.Services;
using ShellMgmt.Domain.UserModels;

namespace ShellMgmt.Api.Controllers.Users;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion(1)]
[Authorize]
public sealed class UserController(ScimService scimService, ILogger<UserController> logger) : ControllerBase
{
    [HttpGet("Get")]
    public async Task<IActionResult> Get(
        [FromQuery] string? search,
        [FromQuery] string? filter,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Fetching users from SCIM: page={Page}, pageSize={PageSize}, search={Search}", page, pageSize, search);
            var users = await scimService.GetUsersAsync(filter: filter, startIndex: (page - 1) * pageSize + 1, count: pageSize);
            logger.LogInformation("SCIM returned {Count} users", users.Count);

            if (!string.IsNullOrEmpty(search))
            {
                users = users.Where(u =>
                    u.UserName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    u.Email.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    u.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    u.LastName.Contains(search, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            return Ok(new { items = users, total = users.Count });
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "HTTP error fetching users from SCIM");
            return StatusCode(502, new { error = $"SCIM API unreachable: {ex.Message}" });
        }
        catch (TaskCanceledException ex)
        {
            logger.LogError(ex, "SCIM request timed out");
            return StatusCode(504, new { error = "SCIM API timed out" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching users: {Message}", ex.Message);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("Get/{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await scimService.GetUserAsync(id);
            if (user == null) return NotFound(new { error = "User not found" });
            return Ok(user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching user {Id}", id);
            return StatusCode(500, new { error = "Failed to retrieve user" });
        }
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] UserDto user, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(user.UserName))
                return BadRequest(new { error = "Username is required" });
            if (string.IsNullOrEmpty(user.Password))
                return BadRequest(new { error = "Password is required" });
            if (string.IsNullOrEmpty(user.FirstName))
                return BadRequest(new { error = "First name is required" });
            if (string.IsNullOrEmpty(user.LastName))
                return BadRequest(new { error = "Last name is required" });
            if (string.IsNullOrEmpty(user.Email))
                return BadRequest(new { error = "Email is required" });

            var result = await scimService.CreateUserAsync(user);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating user");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPut("Update/{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UserDto user, CancellationToken cancellationToken = default)
    {
        try
        {
            var existingUser = await scimService.GetUserAsync(id);
            if (existingUser == null)
                return NotFound(new { error = "User not found" });

            // Only update fields that are provided
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;
            existingUser.Phone = user.Phone;
            existingUser.IsActive = user.IsActive;

            var result = await scimService.UpdateUserAsync(id, existingUser);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating user {Id}", id);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var existingUser = await scimService.GetUserAsync(id);
            if (existingUser == null)
                return NotFound(new { error = "User not found" });

            var result = await scimService.DeleteUserAsync(id);
            if (!result)
                return StatusCode(500, new { error = "Failed to delete user" });

            logger.LogInformation("User {Id} ({Username}) deleted successfully", id, existingUser.UserName);
            return Ok(new { message = "User deleted successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting user {Id}", id);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("GetUserAssignments/{id}")]
    public async Task<IActionResult> GetUserAssignments(string id)
    {
        try
        {
            var assignments = await scimService.GetUserGroupAssignmentsAsync(id);
            return Ok(assignments);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching group assignments for user {Id}", id);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("SyncUserGroups/{id}")]
    public async Task<IActionResult> SyncUserGroups(string id, [FromBody] SyncGroupsRequest request)
    {
        try
        {
            var user = await scimService.GetUserAsync(id);
            if (user == null) return NotFound(new { error = "User not found" });

            await scimService.SyncUserGroupAssignmentsAsync(id, user.UserName, request.GroupIds);
            logger.LogInformation("Group assignments synced for user {Id} ({Username})", id, user.UserName);
            return Ok(new { message = "Group assignments updated successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error syncing groups for user {Id}", id);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("ResetPassword/{id}")]
    public async Task<IActionResult> ResetPassword(string id, [FromBody] ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(request.NewPassword))
                return BadRequest(new { error = "New password is required" });

            var existingUser = await scimService.GetUserAsync(id);
            if (existingUser == null)
                return NotFound(new { error = "User not found" });

            await scimService.ResetPasswordAsync(id, request.NewPassword);
            logger.LogInformation("Password reset for user {Id} ({Username})", id, existingUser.UserName);
            return Ok(new { message = "Password reset successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error resetting password for user {Id}", id);
            return StatusCode(500, new { error = ex.Message });
        }
    }
}

public class ResetPasswordRequest
{
    public string NewPassword { get; set; } = string.Empty;
}

public class SyncGroupsRequest
{
    public List<string> GroupIds { get; set; } = new();
}
