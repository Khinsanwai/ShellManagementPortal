using MediatR;
using ShellMgmt.Domain.AppModels;

namespace ShellMgmt.Application.AppService.Create;

public sealed record CreateAppCommand(
    string Name,
    string Code,
    string? URL,
    string? Icon,
    string? Description,
    string? Version,
    bool Status = true) : IRequest<AppDto>;
