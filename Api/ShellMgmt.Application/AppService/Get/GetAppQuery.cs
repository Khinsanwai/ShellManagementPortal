using MediatR;
using SharedKernel.Domain;
using ShellMgmt.Domain.AppModels;

namespace ShellMgmt.Application.AppService.Get;

public sealed record GetAppQuery(
    int Take = 50,
    int Skip = 1,
    string? SortBy = null,
    string? OrderBy = null,
    int? Id = null,
    string? Name = null) : IRequest<PagedList<AppDto>>;
