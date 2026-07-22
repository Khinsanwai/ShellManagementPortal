using MediatR;
using SharedKernel.Domain;
using ShellMgmt.Application.AppService.AppSpecification;
using ShellMgmt.Domain.AppModels;

namespace ShellMgmt.Application.AppService.Delete;

internal sealed class DeleteAppCommandHandler(IRepository<App> repository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteAppCommand, bool>
{
    public async Task<bool> Handle(DeleteAppCommand command, CancellationToken cancellationToken)
    {
        var spec = new AppByIdSpec(command.Id);
        var entities = await repository.Get(spec, cancellationToken);
        var entity = entities.Items.FirstOrDefault();
        if (entity == null) return false;

        await repository.Delete(entity);
        await unitOfWork.SaveChanges(cancellationToken);

        return true;
    }
}
