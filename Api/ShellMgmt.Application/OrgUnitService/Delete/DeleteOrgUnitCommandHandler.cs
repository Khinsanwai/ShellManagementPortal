using MediatR;
using SharedKernel.Domain;
using ShellMgmt.Domain.OrgUnitModels;

namespace ShellMgmt.Application.OrgUnitService.Delete;

internal sealed class DeleteOrgUnitCommandHandler(IRepository<OrgUnit> repository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteOrgUnitCommand, bool>
{
    public async Task<bool> Handle(DeleteOrgUnitCommand command, CancellationToken cancellationToken)
    {
        var orgUnit = await repository.GetById(command.Id);
        if (orgUnit == null) return false;

        await repository.Delete(orgUnit);
        await unitOfWork.SaveChanges(cancellationToken);

        return true;
    }
}
