using AutoMapper;
using MediatR;
using SharedKernel.Domain;
using ShellMgmt.Domain.OrgUnitModels;

namespace ShellMgmt.Application.OrgUnitService.Create;

internal sealed class CreateOrgUnitCommandHandler(IRepository<OrgUnit> repository, IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<CreateOrgUnitCommand, OrgUnitDto>
{
    public async Task<OrgUnitDto> Handle(CreateOrgUnitCommand command, CancellationToken cancellationToken)
    {
        var orgUnit = new OrgUnit
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            ParentUnitId = command.ParentUnitId,
            Description = command.Description
        };

        await repository.Add(orgUnit);
        await unitOfWork.SaveChanges(cancellationToken);

        return mapper.Map<OrgUnitDto>(orgUnit);
    }
}
