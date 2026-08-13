namespace ShellMgmt.Domain.ApiResourceModels;

public class ApiResourceScopeDto
{
    public string? Id { get; set; }
    public required string Name { get; set; }
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
}
