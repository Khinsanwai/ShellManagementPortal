using MediatR;
using SharedKernel.Domain;
using ShellMgmt.Domain.UserlogModels;

namespace ShellMgmt.Application.UserlogService.Get;

public sealed record GetUserlogQuery(
    int Take = 50,
    int Skip = 1,
    string? SortBy = null,
    string? OrderBy = null,
    long? Id = null,
    string? Username = null,
    string? Category = null,
    string? Action = null,
    string? Module = null,
    string? Application = null,
    string? Result = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null) : IRequest<PagedList<UserlogDto>>;
