using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShellMgmt.Api.Services;
using ShellMgmt.Domain.ApiResourceModels;

namespace ShellMgmt.Api.Controllers.ApiResources;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion(1)]
[Authorize]
public sealed class ApiResourceController(Wso2ApiResourceService apiResourceService, ILogger<ApiResourceController> logger) : ControllerBase
{
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var resources = await apiResourceService.GetApiResourcesAsync();
            return Ok(resources);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching API resources");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("GetById/{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
    {
        try
        {
            var resource = await apiResourceService.GetApiResourceAsync(id);
            if (resource == null) return NotFound(new { error = "API resource not found" });
            return Ok(resource);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching API resource {Id}", id);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] ApiResourceDto apiResource, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(apiResource.Name))
                return BadRequest(new { error = "Name is required" });
            if (string.IsNullOrWhiteSpace(apiResource.Identifier))
                return BadRequest(new { error = "Identifier is required" });

            var result = await apiResourceService.CreateApiResourceAsync(apiResource);
            logger.LogInformation("API resource created: {Name} ({Identifier})", apiResource.Name, apiResource.Identifier);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating API resource");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPut("Update")]
    public async Task<IActionResult> Update([FromBody] ApiResourceDto apiResource, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(apiResource.Id))
                return BadRequest(new { error = "Id is required" });

            var result = await apiResourceService.UpdateApiResourceAsync(apiResource.Id, apiResource);
            logger.LogInformation("API resource updated: {Id}", apiResource.Id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating API resource {Id}", apiResource.Id);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await apiResourceService.DeleteApiResourceAsync(id);
            if (!result) return NotFound(new { error = "API resource not found" });
            logger.LogInformation("API resource deleted: {Id}", id);
            return Ok(new { message = "API resource deleted successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting API resource {Id}", id);
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
