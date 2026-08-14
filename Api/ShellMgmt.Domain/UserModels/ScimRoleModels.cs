using System.Text.Json.Serialization;

namespace ShellMgmt.Domain.UserModels;

public class ScimRoleListResponse
{
    [JsonPropertyName("totalResults")]
    public int TotalResults { get; set; }

    [JsonPropertyName("startIndex")]
    public int StartIndex { get; set; }

    [JsonPropertyName("itemsPerPage")]
    public int ItemsPerPage { get; set; }

    [JsonPropertyName("Resources")]
    public List<ScimRoleResource> Resources { get; set; } = new();
}

public class ScimRoleResource
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("members")]
    public List<ScimRoleMember>? Members { get; set; }

    // WSO2 IS 7.x returns "users" instead of "members" on individual role fetch
    [JsonPropertyName("users")]
    public List<ScimRoleMember>? Users { get; set; }

    [JsonPropertyName("meta")]
    public ScimMeta? Meta { get; set; }
}

public class ScimRoleMember
{
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;

    [JsonPropertyName("display")]
    public string Display { get; set; } = string.Empty;

    [JsonPropertyName("$ref")]
    public string? Ref { get; set; }
}

public class RoleDto
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public List<string> MemberIds { get; set; } = new();
    public List<string> MemberDisplayNames { get; set; } = new();
    public DateTime CreatedDate { get; set; }
    public DateTime LastModifiedDate { get; set; }
}

public class UserRoleAssignment
{
    public string RoleId { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public bool IsAssigned { get; set; }
}

/// <summary>
/// Helper model for SCIM PATCH member operations where $ref must serialize correctly.
/// </summary>
public class ScimPatchMember
{
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;

    [JsonPropertyName("display")]
    public string Display { get; set; } = string.Empty;

    [JsonPropertyName("$ref")]
    public string? Ref { get; set; }
}

public class ScimPatchOperation
{
    [JsonPropertyName("op")]
    public string Op { get; set; } = string.Empty;

    [JsonPropertyName("path")]
    public string? Path { get; set; }

    [JsonPropertyName("value")]
    public object? Value { get; set; }
}

public class ScimPatchRequest
{
    [JsonPropertyName("schemas")]
    public string[] Schemas { get; set; } = ["urn:ietf:params:scim:api:messages:2.0:PatchOp"];

    [JsonPropertyName("Operations")]
    public List<ScimPatchOperation> Operations { get; set; } = new();
}

/// <summary>
/// Represents a permission/scope assigned to a role.
/// </summary>
public class RolePermission
{
    public string RoleName { get; set; } = string.Empty;
    public string ScopeName { get; set; } = string.Empty;
    public string Display { get; set; } = string.Empty;
}
