namespace ShellMgmt.Domain.AppModels;

public class AppDto
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? URL { get; set; }
    public string? Icon { get; set; }
    public string? Description { get; set; }
    public string? Version { get; set; }
    public bool Status { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsVisible { get; set; } = true;
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
