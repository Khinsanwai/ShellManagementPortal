using MediatR;
using SharedKernel.Domain;
using ShellMgmt.Domain.ClaimModels;

namespace ShellMgmt.Application.ClaimService.Get;

public sealed record GetClaimQuery(
    int Take = 50,
    int Skip = 1,
    string? SortBy = null,
    string? OrderBy = null,
    Guid? Id = null,
    string? Name = null) : IRequest<PagedList<ClaimDto>>;
