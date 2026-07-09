namespace ShellMgmt.Domain.OrgUnitModels;

public class OrgUnitDto
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public Guid? ParentUnitId { get; set; }
    public string? Description { get; set; }
}
