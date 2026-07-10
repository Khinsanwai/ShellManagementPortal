using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ShellMgmt.Domain.UserModels;

namespace ShellMgmt.Api.Services;

public class ScimService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly ILogger<ScimService> _logger;

    public ScimService(IConfiguration configuration, ILogger<ScimService> logger)
    {
        _logger = logger;
        _baseUrl = configuration["WSO2:ScimBaseUrl"] ?? "https://localhost:9443/scim2";

        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        };
        _httpClient = new HttpClient(handler);
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/scim+json"));
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "ShellMgmtPortal/1.0");

        var adminUser = configuration["WSO2:AdminUser"] ?? "admin";
        var adminPassword = configuration["WSO2:AdminPassword"] ?? "admin";
        var byteArray = Encoding.ASCII.GetBytes($"{adminUser}:{adminPassword}");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
    }

    public async Task<List<UserDto>> GetUsersAsync(string? filter = null, int startIndex = 1, int count = 100)
    {
        var url = $"{_baseUrl}/Users?startIndex={startIndex}&count={count}";
        if (!string.IsNullOrEmpty(filter))
        {
            url += $"&filter={Uri.EscapeDataString(filter)}";
        }

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var scimResponse = JsonSerializer.Deserialize<ScimListResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return scimResponse?.Resources.Select(MapToUserDto).ToList() ?? new();
    }

    public async Task<UserDto?> GetUserAsync(string userId)
    {
        var url = $"{_baseUrl}/Users/{userId}";
        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode) return null;

        var content = await response.Content.ReadAsStringAsync();
        var scimUser = JsonSerializer.Deserialize<ScimUser>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return scimUser != null ? MapToUserDto(scimUser) : null;
    }

    public async Task<UserDto> CreateUserAsync(UserDto user)
    {
        var scimUser = MapToScimUser(user);
        var json = JsonSerializer.Serialize(scimUser);
        var content = new StringContent(json, Encoding.UTF8, "application/scim+json");

        var response = await _httpClient.PostAsync($"{_baseUrl}/Users", content);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            var error = JsonSerializer.Deserialize<ScimError>(responseBody);
            throw new Exception($"SCIM create user failed: {error?.Detail ?? responseBody}");
        }

        var createdUser = JsonSerializer.Deserialize<ScimUser>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return MapToUserDto(createdUser ?? throw new Exception("Failed to parse SCIM response"));
    }

    public async Task<UserDto> UpdateUserAsync(string userId, UserDto user)
    {
        // SCIM PATCH operation
        var operations = new List<object>();

        if (!string.IsNullOrEmpty(user.FirstName) || !string.IsNullOrEmpty(user.LastName))
        {
            operations.Add(new
            {
                op = "replace",
                value = new
                {
                    name = new
                    {
                        givenName = user.FirstName,
                        familyName = user.LastName
                    }
                }
            });
        }

        if (!string.IsNullOrEmpty(user.Email))
        {
            operations.Add(new
            {
                op = "replace",
                value = new
                {
                    emails = new[]
                    {
                        new { value = user.Email, primary = true, type = "work" }
                    }
                }
            });
        }

        if (!string.IsNullOrEmpty(user.Phone))
        {
            operations.Add(new
            {
                op = "replace",
                value = new
                {
                    phoneNumbers = new[]
                    {
                        new { value = user.Phone, type = "mobile" }
                    }
                }
            });
        }

        // Always include active status
        operations.Add(new
        {
            op = "replace",
            value = new { active = user.IsActive }
        });

        var patchBody = new { schemas = new[] { "urn:ietf:params:scim:api:messages:2.0:PatchOp" }, Operations = operations };
        var json = JsonSerializer.Serialize(patchBody);
        var content = new StringContent(json, Encoding.UTF8, "application/scim+json");

        var request = new HttpRequestMessage(HttpMethod.Patch, $"{_baseUrl}/Users/{userId}")
        {
            Content = content
        };

        var response = await _httpClient.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            var error = JsonSerializer.Deserialize<ScimError>(responseBody);
            throw new Exception($"SCIM update user failed: {error?.Detail ?? responseBody}");
        }

        var updatedUser = JsonSerializer.Deserialize<ScimUser>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return MapToUserDto(updatedUser ?? throw new Exception("Failed to parse SCIM response"));
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        var response = await _httpClient.DeleteAsync($"{_baseUrl}/Users/{userId}");
        return response.IsSuccessStatusCode;
    }

    public async Task<List<GroupDto>> GetGroupsAsync(int startIndex = 1, int count = 100)
    {
        var url = $"{_baseUrl}/Groups?startIndex={startIndex}&count={count}";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var scimResponse = JsonSerializer.Deserialize<ScimGroupListResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return scimResponse?.Resources.Select(MapToGroupDto).ToList() ?? new();
    }

    public async Task<GroupDto?> GetGroupAsync(string groupId)
    {
        var url = $"{_baseUrl}/Groups/{groupId}";
        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode) return null;
        var content = await response.Content.ReadAsStringAsync();
        var scimGroup = JsonSerializer.Deserialize<ScimGroupResource>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return scimGroup != null ? MapToGroupDto(scimGroup) : null;
    }

    public async Task<GroupDto> CreateGroupAsync(string displayName)
    {
        var body = new
        {
            schemas = new[] { "urn:ietf:params:scim:schemas:core:2.0:Group" },
            displayName
        };
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/scim+json");
        var response = await _httpClient.PostAsync($"{_baseUrl}/Groups", content);
        var responseBody = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            var error = JsonSerializer.Deserialize<ScimError>(responseBody);
            throw new Exception($"SCIM create group failed: {error?.Detail ?? responseBody}");
        }
        var created = JsonSerializer.Deserialize<ScimGroupResource>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return MapToGroupDto(created ?? throw new Exception("Failed to parse SCIM group response"));
    }

    public async Task<GroupDto> UpdateGroupAsync(string groupId, string displayName)
    {
        var patchBody = new
        {
            schemas = new[] { "urn:ietf:params:scim:api:messages:2.0:PatchOp" },
            Operations = new[]
            {
                new
                {
                    op = "replace",
                    value = new { displayName }
                }
            }
        };
        var json = JsonSerializer.Serialize(patchBody);
        var content = new StringContent(json, Encoding.UTF8, "application/scim+json");
        var request = new HttpRequestMessage(HttpMethod.Patch, $"{_baseUrl}/Groups/{groupId}") { Content = content };
        var response = await _httpClient.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            var error = JsonSerializer.Deserialize<ScimError>(responseBody);
            throw new Exception($"SCIM update group failed: {error?.Detail ?? responseBody}");
        }
        var updated = JsonSerializer.Deserialize<ScimGroupResource>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return MapToGroupDto(updated ?? throw new Exception("Failed to parse SCIM group response"));
    }

    public async Task<bool> DeleteGroupAsync(string groupId)
    {
        var response = await _httpClient.DeleteAsync($"{_baseUrl}/Groups/{groupId}");
        return response.IsSuccessStatusCode;
    }

    public async Task AssignUserToGroupAsync(string groupId, string userId, string userName)
    {
        // SCIM PATCH to add member
        var patchBody = new
        {
            schemas = new[] { "urn:ietf:params:scim:api:messages:2.0:PatchOp" },
            Operations = new[]
            {
                new
                {
                    op = "add",
                    value = new
                    {
                        members = new[]
                        {
                            new { value = userId, display = userName }
                        }
                    }
                }
            }
        };
        var json = JsonSerializer.Serialize(patchBody);
        var content = new StringContent(json, Encoding.UTF8, "application/scim+json");
        var request = new HttpRequestMessage(HttpMethod.Patch, $"{_baseUrl}/Groups/{groupId}") { Content = content };
        var response = await _httpClient.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            var error = JsonSerializer.Deserialize<ScimError>(responseBody);
            throw new Exception($"SCIM assign group failed: {error?.Detail ?? responseBody}");
        }
    }

    public async Task RemoveUserFromGroupAsync(string groupId, string userId)
    {
        var patchBody = new
        {
            schemas = new[] { "urn:ietf:params:scim:api:messages:2.0:PatchOp" },
            Operations = new[]
            {
                new
                {
                    op = "remove",
                    path = $"members[value eq \"{userId}\"]"
                }
            }
        };
        var json = JsonSerializer.Serialize(patchBody);
        var content = new StringContent(json, Encoding.UTF8, "application/scim+json");
        var request = new HttpRequestMessage(HttpMethod.Patch, $"{_baseUrl}/Groups/{groupId}") { Content = content };
        var response = await _httpClient.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            var error = JsonSerializer.Deserialize<ScimError>(responseBody);
            throw new Exception($"SCIM remove group member failed: {error?.Detail ?? responseBody}");
        }
    }

    public async Task<List<UserGroupAssignment>> GetUserGroupAssignmentsAsync(string userId)
    {
        var groups = await GetGroupsAsync();
        var assignments = new List<UserGroupAssignment>();
        foreach (var group in groups)
        {
            assignments.Add(new UserGroupAssignment
            {
                GroupId = group.Id,
                GroupName = group.DisplayName,
                IsAssigned = group.MemberIds.Contains(userId)
            });
        }
        return assignments;
    }

    public async Task SyncUserGroupAssignmentsAsync(string userId, string userName, List<string> targetGroupIds)
    {
        var currentAssignments = await GetUserGroupAssignmentsAsync(userId);
        foreach (var assignment in currentAssignments)
        {
            var shouldBeAssigned = targetGroupIds.Contains(assignment.GroupId);
            if (assignment.IsAssigned && !shouldBeAssigned)
            {
                await RemoveUserFromGroupAsync(assignment.GroupId, userId);
                _logger.LogInformation("Removed user {UserId} from group {GroupId}", userId, assignment.GroupId);
            }
            else if (!assignment.IsAssigned && shouldBeAssigned)
            {
                await AssignUserToGroupAsync(assignment.GroupId, userId, userName);
                _logger.LogInformation("Assigned user {UserId} to group {GroupId}", userId, assignment.GroupId);
            }
        }
        // Also handle groups that are new and not in current assignments (edge case)
        foreach (var groupId in targetGroupIds)
        {
            if (!currentAssignments.Any(a => a.GroupId == groupId))
            {
                var group = await GetGroupAsync(groupId);
                if (group != null)
                {
                    await AssignUserToGroupAsync(groupId, userId, userName);
                    _logger.LogInformation("Assigned user {UserId} to new group {GroupId}", userId, groupId);
                }
            }
        }
    }

    public async Task ResetPasswordAsync(string userId, string newPassword)
    {
        var patchBody = new
        {
            schemas = new[] { "urn:ietf:params:scim:api:messages:2.0:PatchOp" },
            Operations = new[]
            {
                new
                {
                    op = "replace",
                    value = new { password = newPassword }
                }
            }
        };

        var json = JsonSerializer.Serialize(patchBody);
        var content = new StringContent(json, Encoding.UTF8, "application/scim+json");

        var request = new HttpRequestMessage(HttpMethod.Patch, $"{_baseUrl}/Users/{userId}")
        {
            Content = content
        };

        var response = await _httpClient.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            var error = JsonSerializer.Deserialize<ScimError>(responseBody);
            throw new Exception($"SCIM password reset failed: {error?.Detail ?? responseBody}");
        }
    }

    private static UserDto MapToUserDto(ScimUser scimUser)
    {
        return new UserDto
        {
            Id = scimUser.Id,
            UserName = scimUser.UserName,
            FirstName = scimUser.Name?.GivenName ?? string.Empty,
            LastName = scimUser.Name?.FamilyName ?? string.Empty,
            Email = scimUser.Emails?.FirstOrDefault()?.Value ?? string.Empty,
            Phone = scimUser.PhoneNumbers?.FirstOrDefault()?.Value ?? string.Empty,
            StreetAddress = scimUser.Addresses?.FirstOrDefault()?.StreetAddress ?? string.Empty,
            Locality = scimUser.Addresses?.FirstOrDefault()?.Locality ?? string.Empty,
            Region = scimUser.Addresses?.FirstOrDefault()?.Region ?? string.Empty,
            PostalCode = scimUser.Addresses?.FirstOrDefault()?.PostalCode ?? string.Empty,
            Country = scimUser.Addresses?.FirstOrDefault()?.Country ?? string.Empty,
            IsActive = scimUser.Active,
            Groups = scimUser.Groups?.Select(g => g.Display).ToList() ?? new(),
            CreatedDate = scimUser.Meta?.Created ?? DateTime.MinValue,
            LastModifiedDate = scimUser.Meta?.LastModified ?? DateTime.MinValue
        };
    }

    private static GroupDto MapToGroupDto(ScimGroupResource group)
    {
        return new GroupDto
        {
            Id = group.Id,
            DisplayName = group.DisplayName,
            MemberIds = group.Members?.Select(m => m.Value).ToList() ?? new(),
            MemberDisplayNames = group.Members?.Select(m => m.Display).ToList() ?? new(),
            CreatedDate = group.Meta?.Created ?? DateTime.MinValue,
            LastModifiedDate = group.Meta?.LastModified ?? DateTime.MinValue
        };
    }

    private static ScimUser MapToScimUser(UserDto user)
    {
        var scimUser = new ScimUser
        {
            UserName = user.UserName,
            Name = new ScimName
            {
                GivenName = user.FirstName,
                FamilyName = user.LastName
            },
            Active = user.IsActive,
            Password = string.IsNullOrEmpty(user.Password) ? null : user.Password
        };

        if (!string.IsNullOrEmpty(user.Email))
        {
            scimUser.Emails = new List<ScimEmail>
            {
                new() { Value = user.Email, Primary = true }
            };
        }

        if (!string.IsNullOrEmpty(user.Phone))
        {
            scimUser.PhoneNumbers = new List<ScimPhoneNumber>
            {
                new() { Value = user.Phone, Type = "mobile" }
            };
        }

        return scimUser;
    }
}
