using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ShellMgmt.Domain.ApiResourceModels;

namespace ShellMgmt.Api.Services;

public class Wso2ApiResourceService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly ILogger<Wso2ApiResourceService> _logger;

    public Wso2ApiResourceService(IConfiguration configuration, ILogger<Wso2ApiResourceService> logger)
    {
        _logger = logger;
        _baseUrl = configuration["WSO2:ApiBaseUrl"] ?? "https://localhost:9443/api/server/v1";

        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };
        _httpClient = new HttpClient(handler);
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "ShellMgmtPortal/1.0");

        var adminUser = configuration["WSO2:AdminUser"] ?? "admin";
        var adminPassword = configuration["WSO2:AdminPassword"] ?? "admin";
        var byteArray = Encoding.ASCII.GetBytes($"{adminUser}:{adminPassword}");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
    }

    public async Task<List<ApiResourceDto>> GetApiResourcesAsync()
    {
        var url = $"{_baseUrl}/api-resources";
        _logger.LogInformation("Fetching API resources from {Url}", url);

        var response = await _httpClient.GetAsync(url);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to fetch API resources. Status: {StatusCode}, Body: {Body}", response.StatusCode, responseBody);
            throw new Exception($"Failed to fetch API resources: {responseBody}");
        }

        _logger.LogDebug("API resources response: {Response}", responseBody);
        using var doc = JsonDocument.Parse(responseBody);
        var root = doc.RootElement;

        var resources = new List<ApiResourceDto>();
        if (root.TryGetProperty("apiResources", out var apiResources))
        {
            foreach (var resource in apiResources.EnumerateArray())
            {
                resources.Add(MapToApiResourceDto(resource));
            }
        }
        return resources;
    }

    public async Task<ApiResourceDto?> GetApiResourceAsync(string id)
    {
        var url = $"{_baseUrl}/api-resources/{id}";
        _logger.LogInformation("Fetching API resource {Id} from {Url}", id, url);

        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to fetch API resource: {errorBody}");
        }

        var responseBody = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseBody);
        return MapToApiResourceDto(doc.RootElement);
    }

    public async Task<ApiResourceDto> CreateApiResourceAsync(ApiResourceDto apiResource)
    {
        var url = $"{_baseUrl}/api-resources";
        _logger.LogInformation("Creating API resource {Name} at {Url}", apiResource.Name, url);

        var body = new
        {
            name = apiResource.Name,
            identifier = apiResource.Identifier,
            description = apiResource.Description ?? string.Empty,
            requiresAuthorization = apiResource.RequiresAuthorization,
            scopes = apiResource.Scopes.Select(s => new
            {
                name = s.Name,
                displayName = s.DisplayName ?? s.Name,
                description = s.Description ?? string.Empty
            }).ToArray()
        };

        var json = JsonSerializer.Serialize(body);
        _logger.LogDebug("Create API resource request body: {Body}", json);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to create API resource. Status: {StatusCode}, Body: {Body}", response.StatusCode, responseBody);
            throw new Exception($"Failed to create API resource: {responseBody}");
        }

        _logger.LogInformation("API resource created successfully");
        using var doc = JsonDocument.Parse(responseBody);
        return MapToApiResourceDto(doc.RootElement);
    }

    public async Task<ApiResourceDto> UpdateApiResourceAsync(string id, ApiResourceDto apiResource)
    {
        var url = $"{_baseUrl}/api-resources/{id}";
        _logger.LogInformation("Updating API resource {Id} at {Url}", id, url);

        var body = new
        {
            name = apiResource.Name,
            identifier = apiResource.Identifier,
            description = apiResource.Description ?? string.Empty,
            requiresAuthorization = apiResource.RequiresAuthorization,
            scopes = apiResource.Scopes.Select(s => new
            {
                name = s.Name,
                displayName = s.DisplayName ?? s.Name,
                description = s.Description ?? string.Empty
            }).ToArray()
        };

        var json = JsonSerializer.Serialize(body);
        _logger.LogDebug("Update API resource request body: {Body}", json);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync(url, content);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to update API resource. Status: {StatusCode}, Body: {Body}", response.StatusCode, responseBody);
            throw new Exception($"Failed to update API resource: {responseBody}");
        }

        _logger.LogInformation("API resource {Id} updated successfully", id);
        using var doc = JsonDocument.Parse(responseBody);
        return MapToApiResourceDto(doc.RootElement);
    }

    public async Task<bool> DeleteApiResourceAsync(string id)
    {
        var url = $"{_baseUrl}/api-resources/{id}";
        _logger.LogInformation("Deleting API resource {Id} from {Url}", id, url);

        var response = await _httpClient.DeleteAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            _logger.LogError("Failed to delete API resource. Status: {StatusCode}, Body: {Body}", response.StatusCode, errorBody);
            throw new Exception($"Failed to delete API resource: {errorBody}");
        }

        _logger.LogInformation("API resource {Id} deleted successfully", id);
        return true;
    }

    private static ApiResourceDto MapToApiResourceDto(JsonElement resource)
    {
        var dto = new ApiResourceDto
        {
            Id = resource.TryGetProperty("id", out var id) ? id.GetString() : null,
            Name = resource.TryGetProperty("name", out var name) ? name.GetString() ?? string.Empty : string.Empty,
            Identifier = resource.TryGetProperty("identifier", out var identifier) ? identifier.GetString() ?? string.Empty : string.Empty,
            Description = resource.TryGetProperty("description", out var desc) ? desc.GetString() : null,
            RequiresAuthorization = resource.TryGetProperty("requiresAuthorization", out var reqAuth) && reqAuth.GetBoolean()
        };

        if (resource.TryGetProperty("scopes", out var scopes))
        {
            foreach (var scope in scopes.EnumerateArray())
            {
                dto.Scopes.Add(new ApiResourceScopeDto
                {
                    Id = scope.TryGetProperty("id", out var scopeId) ? scopeId.GetString() : null,
                    Name = scope.TryGetProperty("name", out var scopeName) ? scopeName.GetString() ?? string.Empty : string.Empty,
                    DisplayName = scope.TryGetProperty("displayName", out var displayName) ? displayName.GetString() : null,
                    Description = scope.TryGetProperty("description", out var scopeDesc) ? scopeDesc.GetString() : null
                });
            }
        }

        if (resource.TryGetProperty("meta", out var meta))
        {
            if (meta.TryGetProperty("created", out var created) && created.ValueKind == JsonValueKind.String)
            {
                if (DateTime.TryParse(created.GetString(), out var createdDate))
                    dto.CreatedDate = createdDate;
            }
            if (meta.TryGetProperty("lastModified", out var lastModified) && lastModified.ValueKind == JsonValueKind.String)
            {
                if (DateTime.TryParse(lastModified.GetString(), out var lastModifiedDate))
                    dto.LastModifiedDate = lastModifiedDate;
            }
        }

        return dto;
    }
}
