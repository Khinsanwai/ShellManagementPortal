using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShellMgmt.Domain.MenuItemModels;
using ShellMgmt.Persistence.ApplicationDbContext;

namespace ShellMgmt.Api.Controllers.UserMenus;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion(1)]
[Authorize]
public sealed class UserMenuController(AppDbContext dbContext, ILogger<UserMenuController> logger) : ControllerBase
{
    [HttpGet("GetUserMenus/{wso2UserId}")]
    public async Task<IActionResult> GetUserMenus(string wso2UserId, CancellationToken cancellationToken)
    {
        try
        {
            var assignments = await dbContext.UserMenuAssignment
                .Include(ua => ua.MenuItem)
                .Where(ua => ua.Wso2UserId == wso2UserId)
                .Select(ua => new UserMenuAssignmentDto
                {
                    Id = ua.Id,
                    Wso2UserId = ua.Wso2UserId,
                    Wso2UserName = ua.Wso2UserName,
                    MenuItemId = ua.MenuItemId,
                    MenuItemName = ua.MenuItem != null ? ua.MenuItem.Name : null
                })
                .ToListAsync(cancellationToken);

            return Ok(assignments);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching user menu assignments for {UserId}", wso2UserId);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("GetAllMappings")]
    public async Task<IActionResult> GetAllMappings(CancellationToken cancellationToken)
    {
        try
        {
            var assignments = await dbContext.UserMenuAssignment
                .Include(ua => ua.MenuItem)
                .Select(ua => new UserMenuAssignmentDto
                {
                    Id = ua.Id,
                    Wso2UserId = ua.Wso2UserId,
                    Wso2UserName = ua.Wso2UserName,
                    MenuItemId = ua.MenuItemId,
                    MenuItemName = ua.MenuItem != null ? ua.MenuItem.Name : null
                })
                .ToListAsync(cancellationToken);

            return Ok(assignments);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching all user menu mappings");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("AssignMenus")]
    public async Task<IActionResult> AssignMenus([FromBody] UserMenuMappingRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Wso2UserId))
                return BadRequest(new { error = "Wso2UserId is required" });

            // Remove existing assignments for this user
            var existing = await dbContext.UserMenuAssignment
                .Where(ua => ua.Wso2UserId == request.Wso2UserId)
                .ToListAsync(cancellationToken);

            dbContext.UserMenuAssignment.RemoveRange(existing);

            // Add new assignments
            foreach (var menuItemId in request.MenuItemIds)
            {
                dbContext.UserMenuAssignment.Add(new UserMenuAssignment
                {
                    Id = Guid.NewGuid(),
                    Wso2UserId = request.Wso2UserId,
                    Wso2UserName = request.Wso2UserName,
                    MenuItemId = menuItemId
                });
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Menu assignments updated for user {UserName} ({UserId})", request.Wso2UserName, request.Wso2UserId);
            return Ok(new { message = "Menu assignments updated successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error assigning menus to user {UserId}", request.Wso2UserId);
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
