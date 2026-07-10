namespace ShellMgmt.Domain.MenuItemModels;

public class UserMenuAssignment
{
    public Guid Id { get; set; }
    public required string Wso2UserId { get; set; }
    public required string Wso2UserName { get; set; }
    public Guid MenuItemId { get; set; }
    public MenuItem? MenuItem { get; set; }
}
