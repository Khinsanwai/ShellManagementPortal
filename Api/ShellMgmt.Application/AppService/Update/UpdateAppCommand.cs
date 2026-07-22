using MediatR;
using ShellMgmt.Domain.AppModels;

namespace ShellMgmt.Application.AppService.Update;

public sealed record UpdateAppCommand(
    int Id,
    string Name,
    string Code,
    string? URL,
    string? Icon,
    string? Description,
    string? Version,
    bool Status) : IRequest<AppDto?>;
