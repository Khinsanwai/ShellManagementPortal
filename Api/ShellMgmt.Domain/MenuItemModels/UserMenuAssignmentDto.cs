namespace ShellMgmt.Domain.MenuItemModels;

public class UserMenuAssignmentDto
{
    public Guid? Id { get; set; }
    public string Wso2UserId { get; set; } = string.Empty;
    public string Wso2UserName { get; set; } = string.Empty;
    public Guid MenuItemId { get; set; }
    public string? MenuItemName { get; set; }
}

public class UserMenuMappingRequest
{
    public string Wso2UserId { get; set; } = string.Empty;
    public string Wso2UserName { get; set; } = string.Empty;
    public List<Guid> MenuItemIds { get; set; } = new();
}
