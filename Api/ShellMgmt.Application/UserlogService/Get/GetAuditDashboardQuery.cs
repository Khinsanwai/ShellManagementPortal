using MediatR;
using ShellMgmt.Domain.UserlogModels;

namespace ShellMgmt.Application.UserlogService.Get;

public sealed record GetAuditDashboardQuery : IRequest<AuditDashboardDto>;
