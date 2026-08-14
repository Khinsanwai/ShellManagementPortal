using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShellMgmt.Api.Services;
using ShellMgmt.Domain.AppModels;
using ShellMgmt.Persistence.ApplicationDbContext;

namespace ShellMgmt.Api.Controllers.Apps;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion(1)]
[Authorize]
public sealed class AppGroupController(AppDbContext dbContext, ScimService scimService, ILogger<AppGroupController> logger) : ControllerBase
{
    [HttpGet("GetAppGroups/{appId}")]
    public async Task<IActionResult> GetAppGroups(int appId, CancellationToken cancellationToken)
    {
        try
        {
            var assignments = await dbContext.AppGroupAssignment
                .Where(ag => ag.ApplicationId == appId)
                .Select(ag => new
                {
                    ag.Id,
                    ag.ApplicationId,
                    ag.Wso2GroupId,
                    ag.Wso2GroupName
                })
                .ToListAsync(cancellationToken);

            return Ok(assignments);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching app group assignments for app {AppId}", appId);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("GetUserApps/{wso2UserId}")]
    public async Task<IActionResult> GetUserApps(string wso2UserId, CancellationToken cancellationToken)
    {
        try
        {
            // Get user's groups from WSO2
            var userGroupAssignments = await scimService.GetUserGroupAssignmentsAsync(wso2UserId);
            var assignedGroups = userGroupAssignments.Where(g => g.IsAssigned).ToList();
            var userGroupIds = assignedGroups
                .Select(g => g.GroupId)
                .ToHashSet();

            logger.LogInformation("GetUserApps: UserId={UserId}, AssignedGroups={GroupCount} [{Groups}]",
                wso2UserId, assignedGroups.Count,
                string.Join(", ", assignedGroups.Select(g => $"{g.GroupName}({g.GroupId})")));

            if (userGroupIds.Count == 0)
            {
                logger.LogInformation("GetUserApps: No groups found for user {UserId}, returning empty", wso2UserId);
                return Ok(new List<UserChildApplicationAssignmentDto>());
            }

            // Get all app group assignments in DB for debugging
            var allAssignments = await dbContext.AppGroupAssignment
                .Include(ag => ag.Application)
                .Select(ag => new { ag.Id, ag.ApplicationId, ag.Wso2GroupId, ag.Wso2GroupName, AppName = ag.Application != null ? ag.Application.Name : null })
                .ToListAsync(cancellationToken);

            logger.LogInformation("GetUserApps: All AppGroupAssignments in DB: {Assignments}",
                string.Join("; ", allAssignments.Select(a => $"App={a.AppName}({a.ApplicationId}), Group={a.Wso2GroupName}({a.Wso2GroupId})")));

            // Get apps assigned to those groups
            var apps = await dbContext.AppGroupAssignment
                .Include(ag => ag.Application)
                .Where(ag => userGroupIds.Contains(ag.Wso2GroupId)
                    && ag.Application != null
                    && ag.Application.Status
                    && ag.Application.IsVisible)
                .Select(ag => new UserChildApplicationAssignmentDto
                {
                    ApplicationId = ag.ApplicationId,
                    ApplicationName = ag.Application!.Name,
                    ApplicationUrl = ag.Application.URL,
                    ApplicationIcon = ag.Application.Icon,
                    ApplicationDisplayOrder = ag.Application.DisplayOrder
                })
                .Distinct()
                .OrderBy(a => a.ApplicationDisplayOrder)
                .ToListAsync(cancellationToken);

            logger.LogInformation("GetUserApps: Returning {AppCount} apps for user {UserId}: {Apps}",
                apps.Count, wso2UserId,
                string.Join(", ", apps.Select(a => $"{a.ApplicationName}({a.ApplicationId})")));

            return Ok(apps);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching user apps for user {UserId}", wso2UserId);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var assignments = await dbContext.AppGroupAssignment
                .Include(ag => ag.Application)
                .Select(ag => new
                {
                    ag.Id,
                    ag.ApplicationId,
                    ApplicationName = ag.Application != null ? ag.Application.Name : null,
                    ag.Wso2GroupId,
                    ag.Wso2GroupName
                })
                .ToListAsync(cancellationToken);

            return Ok(assignments);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching all app group assignments");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("SyncAppGroups/{appId}")]
    public async Task<IActionResult> SyncAppGroups(int appId, [FromBody] SyncAppGroupsRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var app = await dbContext.App.FirstOrDefaultAsync(a => a.Id == appId, cancellationToken);
            if (app == null)
                return NotFound(new { error = "Application not found" });

            // Remove existing assignments for this app
            var existing = await dbContext.AppGroupAssignment
                .Where(ag => ag.ApplicationId == appId)
                .ToListAsync(cancellationToken);

            dbContext.AppGroupAssignment.RemoveRange(existing);

            // Add new assignments
            foreach (var group in request.Groups)
            {
                dbContext.AppGroupAssignment.Add(new AppGroupAssignment
                {
                    Id = Guid.NewGuid(),
                    ApplicationId = appId,
                    Wso2GroupId = group.GroupId,
                    Wso2GroupName = group.GroupName
                });
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation("App group assignments updated for app {AppName} ({AppId}). {Count} groups assigned.",
                app.Name, appId, request.Groups.Count);
            return Ok(new { message = "App group assignments updated successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error syncing app group assignments for app {AppId}", appId);
            return StatusCode(500, new { error = ex.InnerException?.Message ?? ex.Message });
        }
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var assignment = await dbContext.AppGroupAssignment
                .FirstOrDefaultAsync(ag => ag.Id == id, cancellationToken);

            if (assignment == null)
                return NotFound(new { error = "Assignment not found" });

            dbContext.AppGroupAssignment.Remove(assignment);
            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("App group assignment {Id} deleted", id);
            return Ok(new { message = "Assignment deleted successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting app group assignment {Id}", id);
            return StatusCode(500, new { error = ex.Message });
        }
    }
}

public class SyncAppGroupsRequest
{
    public List<GroupItem> Groups { get; set; } = new();
}

public class GroupItem
{
    public string GroupId { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
}
