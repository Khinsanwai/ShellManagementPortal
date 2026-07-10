using MediatR;

namespace ShellMgmt.Application.MenuItemService.Delete;

public sealed record DeleteMenuItemCommand(Guid Id) : IRequest<bool>;
