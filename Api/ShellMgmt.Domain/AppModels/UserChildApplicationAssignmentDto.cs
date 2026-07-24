namespace ShellMgmt.Domain.AppModels;

public class UserChildApplicationAssignmentDto
{
    public Guid? Id { get; set; }
    public string Wso2UserId { get; set; } = string.Empty;
    public string Wso2UserName { get; set; } = string.Empty;
    public int ApplicationId { get; set; }
    public string? ApplicationName { get; set; }
}

public class UserChildAppMappingRequest
{
    public string Wso2UserId { get; set; } = string.Empty;
    public string Wso2UserName { get; set; } = string.Empty;
    public List<int> ApplicationIds { get; set; } = new();
}
