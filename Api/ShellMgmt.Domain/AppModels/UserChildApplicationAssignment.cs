namespace ShellMgmt.Domain.AppModels;

public class UserChildApplicationAssignment
{
    public Guid Id { get; set; }
    public required string Wso2UserId { get; set; }
    public required string Wso2UserName { get; set; }
    public int ApplicationId { get; set; }
    public App? Application { get; set; }
}
