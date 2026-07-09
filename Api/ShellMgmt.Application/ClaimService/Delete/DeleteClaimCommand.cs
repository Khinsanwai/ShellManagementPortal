using MediatR;

namespace ShellMgmt.Application.ClaimService.Delete;

public sealed record DeleteClaimCommand(Guid Id) : IRequest<bool>;
