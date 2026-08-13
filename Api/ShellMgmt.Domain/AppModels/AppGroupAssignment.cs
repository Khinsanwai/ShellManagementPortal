namespace ShellMgmt.Domain.AppModels;

public class AppGroupAssignment
{
    public Guid Id { get; set; }
    public int ApplicationId { get; set; }
    public App? Application { get; set; }
    public required string Wso2GroupId { get; set; }
    public required string Wso2GroupName { get; set; }
}
