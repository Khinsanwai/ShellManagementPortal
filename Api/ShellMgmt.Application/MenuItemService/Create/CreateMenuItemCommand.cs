using MediatR;
using ShellMgmt.Domain.MenuItemModels;

namespace ShellMgmt.Application.MenuItemService.Create;

public sealed record CreateMenuItemCommand(string Name, string? Url, Guid? ParentId, int? MenuOrder) : IRequest<MenuItemDto>;
