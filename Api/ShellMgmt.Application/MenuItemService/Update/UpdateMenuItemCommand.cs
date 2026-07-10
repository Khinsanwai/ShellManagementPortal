using MediatR;
using ShellMgmt.Domain.MenuItemModels;

namespace ShellMgmt.Application.MenuItemService.Update;

public sealed record UpdateMenuItemCommand(Guid Id, string Name, string? Url, Guid? ParentId, int? MenuOrder) : IRequest<MenuItemDto?>;
