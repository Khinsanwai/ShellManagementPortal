namespace ShellMgmt.Domain.AppModels;

public class App
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Code { get; set; }
    public string? URL { get; set; }
    public string? Icon { get; set; }
    public string? Description { get; set; }
    public string? Version { get; set; }
    public bool Status { get; set; } = true;
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
