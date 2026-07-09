namespace ShellMgmt.Domain.ResourceModels;

public class Resource
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public Guid ApplicationId { get; set; }
    public Guid TenantId { get; set; }
    public Guid? MenuItemId { get; set; }
}
