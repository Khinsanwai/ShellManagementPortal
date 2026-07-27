using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShellMgmt.Domain.AppModels;
using ShellMgmt.Persistence.ApplicationDbContext;

namespace ShellMgmt.Api.Controllers.Apps;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion(1)]
[Authorize]
public sealed class UserChildApplicationController(AppDbContext dbContext, ILogger<UserChildApplicationController> logger) : ControllerBase
{
    [HttpGet("GetUserApps/{wso2UserId}")]
    public async Task<IActionResult> GetUserApps(string wso2UserId, CancellationToken cancellationToken)
    {
        try
        {
            var assignments = await dbContext.UserChildApplicationAssignment
                .Include(ua => ua.Application)
                .Where(ua => ua.Wso2UserId == wso2UserId && ua.Application != null && ua.Application.Status && ua.Application.IsVisible)
                .OrderBy(ua => ua.Application != null ? ua.Application.DisplayOrder : 0)
                .Select(ua => new UserChildApplicationAssignmentDto
                {
                    Id = ua.Id,
                    Wso2UserId = ua.Wso2UserId,
                    Wso2UserName = ua.Wso2UserName,
                    ApplicationId = ua.ApplicationId,
                    ApplicationName = ua.Application != null ? ua.Application.Name : null,
                    ApplicationUrl = ua.Application != null ? ua.Application.URL : null,
                    ApplicationIcon = ua.Application != null ? ua.Application.Icon : null,
                    ApplicationDisplayOrder = ua.Application != null ? ua.Application.DisplayOrder : 0
                })
                .ToListAsync(cancellationToken);

            return Ok(assignments);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching user child application assignments for {UserId}", wso2UserId);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var assignments = await dbContext.UserChildApplicationAssignment
                .Include(ua => ua.Application)
                .Select(ua => new UserChildApplicationAssignmentDto
                {
                    Id = ua.Id,
                    Wso2UserId = ua.Wso2UserId,
                    Wso2UserName = ua.Wso2UserName,
                    ApplicationId = ua.ApplicationId,
                    ApplicationName = ua.Application != null ? ua.Application.Name : null
                })
                .ToListAsync(cancellationToken);

            return Ok(assignments);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching all user child application assignments");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var assignment = await dbContext.UserChildApplicationAssignment
                .FirstOrDefaultAsync(ua => ua.Id == id, cancellationToken);

            if (assignment == null)
                return NotFound(new { error = "Assignment not found" });

            dbContext.UserChildApplicationAssignment.Remove(assignment);
            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Child application assignment {Id} deleted", id);
            return Ok(new { message = "Assignment deleted successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting child application assignment {Id}", id);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("AssignApps")]
    public async Task<IActionResult> AssignApps([FromBody] UserChildAppMappingRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Wso2UserId))
                return BadRequest(new { error = "Wso2UserId is required" });

            // Remove existing assignments for this user
            var existing = await dbContext.UserChildApplicationAssignment
                .Where(ua => ua.Wso2UserId == request.Wso2UserId)
                .ToListAsync(cancellationToken);

            dbContext.UserChildApplicationAssignment.RemoveRange(existing);

            // Add new assignments
            foreach (var applicationId in request.ApplicationIds)
            {
                dbContext.UserChildApplicationAssignment.Add(new UserChildApplicationAssignment
                {
                    Id = Guid.NewGuid(),
                    Wso2UserId = request.Wso2UserId,
                    Wso2UserName = request.Wso2UserName,
                    ApplicationId = applicationId
                });
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Child application assignments updated for user {UserName} ({UserId})", request.Wso2UserName, request.Wso2UserId);
            return Ok(new { message = "Child application assignments updated successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error assigning child applications to user {UserId}", request.Wso2UserId);
            return StatusCode(500, new { error = ex.InnerException?.Message ?? ex.Message });
        }
    }
}
