using AutoMapper;
using MediatR;
using SharedKernel.Domain;
using ShellMgmt.Domain.InstitutionModels;

namespace ShellMgmt.Application.InstitutionService.Create;

internal sealed class CreateInstitutionCommandHandler(IRepository<Institution> repository, IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<CreateInstitutionCommand, InstitutionDto>
{
    public async Task<InstitutionDto> Handle(CreateInstitutionCommand command, CancellationToken cancellationToken)
    {
        var institution = new Institution
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Code = command.Code,
            Description = command.Description
        };

        await repository.Add(institution);
        await unitOfWork.SaveChanges(cancellationToken);

        return mapper.Map<InstitutionDto>(institution);
    }
}
