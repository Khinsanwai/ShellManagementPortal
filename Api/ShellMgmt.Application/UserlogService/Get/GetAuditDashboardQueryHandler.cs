using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ShellMgmt.Domain.UserlogModels;
using ShellMgmt.Persistence.ApplicationDbContext;

namespace ShellMgmt.Application.UserlogService.Get;

internal sealed class GetAuditDashboardQueryHandler(ReadDbContext dbContext, IMapper mapper)
    : IRequestHandler<GetAuditDashboardQuery, AuditDashboardDto>
{
    public async Task<AuditDashboardDto> Handle(GetAuditDashboardQuery request, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var thirtyMinutesAgo = DateTime.UtcNow.AddMinutes(-30);

        var totalLogins = await dbContext.Userlog
            .CountAsync(x => x.Category == "Login" && x.Result == "Success", cancellationToken);

        var failedLogins = await dbContext.Userlog
            .CountAsync(x => x.Category == "FailedLogin" ||
                             (x.Category == "Login" && x.Result == "Failed"), cancellationToken);

        var onlineUsers = await dbContext.Userlog
            .Where(x => x.CreatedDate >= thirtyMinutesAgo && x.UserId != null)
            .Select(x => x.UserId)
            .Distinct()
            .CountAsync(cancellationToken);

        var adminActivitiesToday = await dbContext.Userlog
            .CountAsync(x => x.CreatedDate >= today, cancellationToken);

        var tokensIssued = await dbContext.Userlog
            .CountAsync(x => x.Category == "TokenIssued", cancellationToken);

        var recentLogs = await dbContext.Userlog
            .OrderByDescending(x => x.CreatedDate)
            .Take(10)
            .ToListAsync(cancellationToken);

        var recentActivities = mapper.Map<List<UserlogDto>>(recentLogs);

        return new AuditDashboardDto
        {
            TotalLogins = totalLogins,
            FailedLogins = failedLogins,
            OnlineUsers = onlineUsers,
            AdminActivitiesToday = adminActivitiesToday,
            TokensIssued = tokensIssued,
            RecentActivities = recentActivities
        };
    }
}
