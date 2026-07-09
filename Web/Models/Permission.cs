namespace ShellMgmt.Web.Models;

public class Permission
{
    public string Rsid { get; set; } = string.Empty;
    public string Rsname { get; set; } = string.Empty;
    public List<string> Scopes { get; set; } = [];
}
