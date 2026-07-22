using MediatR;
using ShellMgmt.Domain.UserlogModels;

namespace ShellMgmt.Application.UserlogService.Create;

public sealed record CreateUserlogCommand(
    string? UserId,
    string? Username,
    string Category,
    string Action,
    string? Application,
    string? Module,
    string? Description,
    string Result,
    string? IPAddress,
    string? UserAgent) : IRequest<UserlogDto>;
