namespace ShellMgmt.Domain.ClaimModels;

public class Claim
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Value { get; set; }
    public string? Description { get; set; }
}
