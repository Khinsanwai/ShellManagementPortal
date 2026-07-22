namespace ShellMgmt.Domain.UserlogModels;

public class UserlogDto
{
    public long Id { get; set; }
    public string? UserId { get; set; }
    public string? Username { get; set; }
    public string? Category { get; set; }
    public string? Action { get; set; }
    public string? Application { get; set; }
    public string? Module { get; set; }
    public string? Description { get; set; }
    public string? Result { get; set; }
    public string? IPAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime CreatedDate { get; set; }
}
