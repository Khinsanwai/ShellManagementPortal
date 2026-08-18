namespace ShellMgmt.Domain.ApiResourceModels;

public class ApiResourceDto
{
    public string? Id { get; set; }
    public required string Name { get; set; }
    public required string Identifier { get; set; }
    public string? Description { get; set; }
    public string? Type { get; set; }
    public bool RequiresAuthorization { get; set; }
    public List<ApiResourceScopeDto> Scopes { get; set; } = new();
    public DateTime? CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
}
