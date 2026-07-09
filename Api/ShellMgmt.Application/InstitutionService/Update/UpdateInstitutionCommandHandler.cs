using AutoMapper;
using MediatR;
using SharedKernel.Domain;
using ShellMgmt.Domain.InstitutionModels;

namespace ShellMgmt.Application.InstitutionService.Update;

internal sealed class UpdateInstitutionCommandHandler(IRepository<Institution> repository, IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<UpdateInstitutionCommand, InstitutionDto?>
{
    public async Task<InstitutionDto?> Handle(UpdateInstitutionCommand command, CancellationToken cancellationToken)
    {
        var institution = await repository.GetById(command.Id);
        if (institution == null) return null;

        institution.Name = command.Name;
        institution.Code = command.Code;
        institution.Description = command.Description;

        await repository.Update(institution);
        await unitOfWork.SaveChanges(cancellationToken);

        return mapper.Map<InstitutionDto>(institution);
    }
}
