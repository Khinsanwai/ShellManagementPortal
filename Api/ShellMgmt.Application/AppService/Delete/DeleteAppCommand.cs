using MediatR;

namespace ShellMgmt.Application.AppService.Delete;

public sealed record DeleteAppCommand(int Id) : IRequest<bool>;
