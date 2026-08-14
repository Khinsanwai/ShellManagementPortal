namespace ShellMgmt.Domain.UserlogModels;

public class Userlog
{
    public long Id { get; set; }
    public string? UserId { get; set; }
    public string? Username { get; set; }
    public required string Category { get; set; }
    public required string Action { get; set; }
    public string? Application { get; set; }
    public string? Module { get; set; }
    public string? Description { get; set; }
    public required string Result { get; set; }
    public string? IPAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime CreatedDate { get; set; }
}
