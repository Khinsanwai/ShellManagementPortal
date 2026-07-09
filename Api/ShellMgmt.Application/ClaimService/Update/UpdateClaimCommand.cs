using MediatR;
using ShellMgmt.Domain.ClaimModels;

namespace ShellMgmt.Application.ClaimService.Update;

public sealed record UpdateClaimCommand(Guid Id, string Name, string? Value, string? Description) : IRequest<ClaimDto?>;
