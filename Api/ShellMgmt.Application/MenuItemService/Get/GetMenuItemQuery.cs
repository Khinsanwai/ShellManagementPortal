using MediatR;
using SharedKernel.Domain;
using ShellMgmt.Domain.MenuItemModels;

namespace ShellMgmt.Application.MenuItemService.Get;

public sealed record GetMenuItemQuery(
    int Take = 50,
    int Skip = 1,
    string? SortBy = null,
    string? OrderBy = null,
    Guid? Id = null,
    string? Name = null,
    string? Url = null,
    Guid? ParentId = null,
    int? MenuOrder = null) : IRequest<PagedList<MenuItemDto>>;
