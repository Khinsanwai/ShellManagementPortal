using System.Text.Json.Serialization;

namespace ShellMgmt.Web.Models;

// SCIM2 Roles Response models
public class ScimRolesResponse
{
    [JsonPropertyName("Resources")]
    public List<ScimRole> Resources { get; set; } = new();
}

public class ScimRole
{
    [JsonPropertyName("displayName")]
    public string? DisplayName { get; set; }

    [JsonPropertyName("permissions")]
    public List<ScimPermission>? Permissions { get; set; }
}

public class ScimPermission
{
    [JsonPropertyName("value")]
    public string? Value { get; set; }

    [JsonPropertyName("display")]
    public string? Display { get; set; }
}

// SCIM2 User Response models
public class ScimUserResponse
{
    [JsonPropertyName("groups")]
    public List<ScimUserGroup>? Groups { get; set; }
}

public class ScimUserGroup
{
    [JsonPropertyName("value")]
    public string? Value { get; set; }

    [JsonPropertyName("display")]
    public string? Display { get; set; }
}

// Role Permission model for API response
public class RolePermission
{
    public string RoleName { get; set; } = string.Empty;
    public string ScopeName { get; set; } = string.Empty;
    public string Display { get; set; } = string.Empty;
}
