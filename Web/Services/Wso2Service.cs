using System.Net.Http.Headers;
using System.Text.Json;
using ShellMgmt.Web.Models;

namespace ShellMgmt.Web.Services;

public class Wso2Service(HttpClient httpClient, IConfiguration config, ILogger<Wso2Service> logger)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly IConfiguration _config = config;
    private readonly ILogger<Wso2Service> _logger = logger;

    public async Task SessionClear(string refToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _config["WSO2:RevokeEndPoint"]);
        var formData = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("client_id", _config["WSO2:ClientId"]!),
            new KeyValuePair<string, string>("client_secret", _config["WSO2:ClientSecret"]!),
            new KeyValuePair<string, string>("token", refToken),
        ]);

        request.Content = formData;
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    public async Task<string?> GetRptAsync(string token)
    {
        var tokenEndpoint = _config["WSO2:TokenEndPoint"];
        _logger.LogInformation("WSO2 TokenEndPoint: {Endpoint}", tokenEndpoint);

        var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);

        var formData = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("grant_type", "urn:ietf:params:oauth:grant-type:uma-ticket"),
            new KeyValuePair<string, string>("audience", _config["WSO2:ClientId"]!),
        ]);

        request.Content = formData;

        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(content);
            return tokenResponse?.AccessToken;
        }

        var errorBody = await response.Content.ReadAsStringAsync();
        _logger.LogWarning("Failed to get RPT. Status: {StatusCode}, Body: {ErrorBody}", response.StatusCode, errorBody);
        return null;
    }

    /// <summary>
    /// Get role details including assigned permissions from WSO2 SCIM2 API
    /// </summary>
    public async Task<List<RolePermission>> GetRolePermissionsAsync(string roleName)
    {
        var permissions = new List<RolePermission>();
        var scimBaseUrl = GetScimBaseUrl();

        try
        {
            var adminToken = await GetAdminAccessTokenAsync();
            if (string.IsNullOrEmpty(adminToken))
            {
                _logger.LogWarning("Failed to get admin access token for role permission lookup");
                return permissions;
            }

            var scimUrl = $"{scimBaseUrl}/Roles?filter=displayName eq \"{roleName}\"";
            var request = new HttpRequestMessage(HttpMethod.Get, scimUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("SCIM2 Roles response: {Content}", content);

                var scimResponse = JsonSerializer.Deserialize<ScimRolesResponse>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (scimResponse?.Resources != null)
                {
                    foreach (var resource in scimResponse.Resources)
                    {
                        if (resource.Permissions != null)
                        {
                            foreach (var permission in resource.Permissions)
                            {
                                permissions.Add(new RolePermission
                                {
                                    RoleName = roleName,
                                    ScopeName = permission.Value ?? string.Empty,
                                    Display = permission.Display ?? permission.Value ?? string.Empty
                                });
                            }
                        }
                    }
                }
            }
            else
            {
                _logger.LogWarning("SCIM2 Roles request failed. Status: {StatusCode}", await response.Content.ReadAsStringAsync());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching role permissions for role: {RoleName}", roleName);
        }

        return permissions;
    }

    /// <summary>
    /// Get all roles assigned to a user and their permissions
    /// </summary>
    public async Task<List<RolePermission>> GetUserRolePermissionsAsync(string wso2UserId)
    {
        var allPermissions = new List<RolePermission>();
        var scimBaseUrl = GetScimBaseUrl();

        try
        {
            var adminToken = await GetAdminAccessTokenAsync();
            if (string.IsNullOrEmpty(adminToken))
            {
                return allPermissions;
            }

            var userUrl = $"{scimBaseUrl}/Users/{wso2UserId}?attributes=groups";
            var request = new HttpRequestMessage(HttpMethod.Get, userUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var userResponse = JsonSerializer.Deserialize<ScimUserResponse>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (userResponse?.Groups != null)
                {
                    foreach (var group in userResponse.Groups)
                    {
                        if (!string.IsNullOrEmpty(group.Value))
                        {
                            var rolePermissions = await GetRolePermissionsAsync(group.Value);
                            allPermissions.AddRange(rolePermissions);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user role permissions for user: {UserId}", wso2UserId);
        }

        return allPermissions;
    }

    private string GetScimBaseUrl()
    {
        return _config["WSO2:ScimBaseUrl"]?.TrimEnd('/')
            ?? throw new InvalidOperationException("WSO2:ScimBaseUrl is not configured in appsettings.");
    }

    private async Task<string?> GetAdminAccessTokenAsync()
    {
        try
        {
            var tokenUrl = _config["WSO2:TokenEndPoint"]
                ?? throw new InvalidOperationException("WSO2:TokenEndPoint is not configured in appsettings.");
            var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl);

            var formData = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("client_id", _config["WSO2:ClientId"]!),
                new KeyValuePair<string, string>("client_secret", _config["WSO2:ClientSecret"]!),
                new KeyValuePair<string, string>("username", _config["WSO2:AdminUser"]!),
                new KeyValuePair<string, string>("password", _config["WSO2:AdminPassword"]!),
                new KeyValuePair<string, string>("scope", "openid"),
            ]);

            request.Content = formData;

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return tokenResponse?.AccessToken;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get admin access token");
        }

        return null;
    }
}
