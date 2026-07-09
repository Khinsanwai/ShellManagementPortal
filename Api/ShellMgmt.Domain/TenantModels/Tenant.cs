namespace ShellMgmt.Domain.TenantModels;

public class Tenant
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
}
