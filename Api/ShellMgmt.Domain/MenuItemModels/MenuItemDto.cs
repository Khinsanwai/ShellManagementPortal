namespace ShellMgmt.Domain.MenuItemModels;

public class MenuItemDto
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public string? Url { get; set; }
    public Guid? ParentId { get; set; }
    public int? MenuOrder { get; set; }
}
