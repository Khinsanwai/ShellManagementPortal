namespace ShellMgmt.Domain.UserlogModels;

public class AuditDashboardDto
{
    public int TotalLogins { get; set; }
    public int FailedLogins { get; set; }
    public int OnlineUsers { get; set; }
    public int AdminActivitiesToday { get; set; }
    public int TokensIssued { get; set; }
    public List<UserlogDto> RecentActivities { get; set; } = new();
}
