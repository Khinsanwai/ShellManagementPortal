namespace ShellMgmt.Domain.OrgUnitModels;

public class OrgUnit
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public Guid? ParentUnitId { get; set; }
    public string? Description { get; set; }
}
