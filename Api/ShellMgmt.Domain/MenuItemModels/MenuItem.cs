namespace ShellMgmt.Domain.MenuItemModels;

public class MenuItem
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Url { get; set; }
    public Guid? ParentId { get; set; }
    public int? MenuOrder { get; set; }
}
