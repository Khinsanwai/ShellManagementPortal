namespace ShellMgmt.Domain.ResourceModels;

public class ResourceDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public Guid? MenuItemId { get; set; }
}
