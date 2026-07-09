using AutoMapper;
using MediatR;
using SharedKernel.Domain;
using ShellMgmt.Domain.OrgUnitModels;

namespace ShellMgmt.Application.OrgUnitService.Update;

internal sealed class UpdateOrgUnitCommandHandler(IRepository<OrgUnit> repository, IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<UpdateOrgUnitCommand, OrgUnitDto?>
{
    public async Task<OrgUnitDto?> Handle(UpdateOrgUnitCommand command, CancellationToken cancellationToken)
    {
        var orgUnit = await repository.GetById(command.Id);
        if (orgUnit == null) return null;

        orgUnit.Name = command.Name;
        orgUnit.ParentUnitId = command.ParentUnitId;
        orgUnit.Description = command.Description;

        await repository.Update(orgUnit);
        await unitOfWork.SaveChanges(cancellationToken);

        return mapper.Map<OrgUnitDto>(orgUnit);
    }
}
