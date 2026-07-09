using MediatR;
using ShellMgmt.Domain.ClaimModels;

namespace ShellMgmt.Application.ClaimService.Create;

public sealed record CreateClaimCommand(string Name, string? Value, string? Description) : IRequest<ClaimDto>;
