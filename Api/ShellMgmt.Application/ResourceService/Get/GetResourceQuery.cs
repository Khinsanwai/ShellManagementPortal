using MediatR;
using SharedKernel.Domain;
using ShellMgmt.Domain.ResourceModels;

namespace ShellMgmt.Application.ResourceService.Get;

public sealed record GetResourceQuery(
    int Take = 50,
    int Skip = 1,
    string? SortBy = null,
    string? OrderBy = null,
    Guid? Id = null,
    string? Name = null,
    string? Description = null,
    Guid? MenuItemId = null) : IRequest<PagedList<ResourceDto>>;
