using MediatR;
using SharedKernel.Domain;
using ShellMgmt.Domain.InstitutionModels;

namespace ShellMgmt.Application.InstitutionService.Get;

public sealed record GetInstitutionQuery(
    int Take = 50,
    int Skip = 1,
    string? SortBy = null,
    string? OrderBy = null,
    Guid? Id = null,
    string? Name = null) : IRequest<PagedList<InstitutionDto>>;
