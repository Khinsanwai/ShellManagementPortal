namespace ShellMgmt.Domain.InstitutionModels;

public class Institution
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
}
