namespace ShellMgmt.Domain.AppModels;

public class App
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
}
