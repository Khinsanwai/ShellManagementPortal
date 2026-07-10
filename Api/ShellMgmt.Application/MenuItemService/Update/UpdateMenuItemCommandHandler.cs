using AutoMapper;
using MediatR;
using SharedKernel.Domain;
using ShellMgmt.Domain.MenuItemModels;

namespace ShellMgmt.Application.MenuItemService.Update;

internal sealed class UpdateMenuItemCommandHandler(IRepository<MenuItem> repository, IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<UpdateMenuItemCommand, MenuItemDto?>
{
    public async Task<MenuItemDto?> Handle(UpdateMenuItemCommand command, CancellationToken cancellationToken)
    {
        var menuItem = await repository.GetById(command.Id);
        if (menuItem == null) return null;

        menuItem.Name = command.Name;
        menuItem.Url = command.Url;
        menuItem.ParentId = command.ParentId;
        menuItem.MenuOrder = command.MenuOrder;

        await repository.Update(menuItem);
        await unitOfWork.SaveChanges(cancellationToken);

        return mapper.Map<MenuItemDto>(menuItem);
    }
}
