using MediatR;
using SharedKernel.Domain;
using ShellMgmt.Domain.MenuItemModels;

namespace ShellMgmt.Application.MenuItemService.Delete;

internal sealed class DeleteMenuItemCommandHandler(IRepository<MenuItem> repository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteMenuItemCommand, bool>
{
    public async Task<bool> Handle(DeleteMenuItemCommand command, CancellationToken cancellationToken)
    {
        var menuItem = await repository.GetById(command.Id);
        if (menuItem == null) return false;

        await repository.Delete(menuItem);
        await unitOfWork.SaveChanges(cancellationToken);

        return true;
    }
}
