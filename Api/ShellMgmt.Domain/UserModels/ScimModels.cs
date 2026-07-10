using System.Text.Json.Serialization;

namespace ShellMgmt.Domain.UserModels;

public class ScimListResponse
{
    [JsonPropertyName("totalResults")]
    public int TotalResults { get; set; }

    [JsonPropertyName("startIndex")]
    public int StartIndex { get; set; }

    [JsonPropertyName("itemsPerPage")]
    public int ItemsPerPage { get; set; }

    [JsonPropertyName("Resources")]
    public List<ScimUser> Resources { get; set; } = new();
}

public class ScimUser
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("userName")]
    public string UserName { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public ScimName? Name { get; set; }

    [JsonPropertyName("emails")]
    public List<ScimEmail>? Emails { get; set; }

    [JsonPropertyName("phoneNumbers")]
    public List<ScimPhoneNumber>? PhoneNumbers { get; set; }

    [JsonPropertyName("addresses")]
    public List<ScimAddress>? Addresses { get; set; }

    [JsonPropertyName("groups")]
    public List<ScimGroup>? Groups { get; set; }

    [JsonPropertyName("active")]
    public bool Active { get; set; }

    [JsonPropertyName("meta")]
    public ScimMeta? Meta { get; set; }

    [JsonPropertyName("password")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Password { get; set; }
}

public class ScimName
{
    [JsonPropertyName("givenName")]
    public string GivenName { get; set; } = string.Empty;

    [JsonPropertyName("familyName")]
    public string FamilyName { get; set; } = string.Empty;
}

public class ScimEmail
{
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;

    [JsonPropertyName("primary")]
    public bool Primary { get; set; }
}

public class ScimPhoneNumber
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "mobile";

    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
}

public class ScimAddress
{
    [JsonPropertyName("streetAddress")]
    public string? StreetAddress { get; set; }

    [JsonPropertyName("locality")]
    public string? Locality { get; set; }

    [JsonPropertyName("region")]
    public string? Region { get; set; }

    [JsonPropertyName("postalCode")]
    public string? PostalCode { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }
}

public class ScimGroup
{
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;

    [JsonPropertyName("display")]
    public string Display { get; set; } = string.Empty;

    [JsonPropertyName("$ref")]
    public string Ref { get; set; } = string.Empty;
}

public class ScimMeta
{
    [JsonPropertyName("created")]
    public DateTime Created { get; set; }

    [JsonPropertyName("lastModified")]
    public DateTime LastModified { get; set; }
}

public class ScimError
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("detail")]
    public string Detail { get; set; } = string.Empty;
}
