using MediatR;
using SharedKernel.Domain;
using ShellMgmt.Domain.OrgUnitModels;

namespace ShellMgmt.Application.OrgUnitService.Get;

public sealed record GetOrgUnitQuery(
    int Take = 50,
    int Skip = 1,
    string? SortBy = null,
    string? OrderBy = null,
    Guid? Id = null,
    string? Name = null) : IRequest<PagedList<OrgUnitDto>>;
