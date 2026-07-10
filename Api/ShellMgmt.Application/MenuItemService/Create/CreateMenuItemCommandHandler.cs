using AutoMapper;
using MediatR;
using SharedKernel.Domain;
using ShellMgmt.Domain.MenuItemModels;

namespace ShellMgmt.Application.MenuItemService.Create;

internal sealed class CreateMenuItemCommandHandler(IRepository<MenuItem> repository, IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<CreateMenuItemCommand, MenuItemDto>
{
    public async Task<MenuItemDto> Handle(CreateMenuItemCommand command, CancellationToken cancellationToken)
    {
        var menuItem = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Url = command.Url,
            ParentId = command.ParentId,
            MenuOrder = command.MenuOrder
        };

        await repository.Add(menuItem);
        await unitOfWork.SaveChanges(cancellationToken);

        return mapper.Map<MenuItemDto>(menuItem);
    }
}
