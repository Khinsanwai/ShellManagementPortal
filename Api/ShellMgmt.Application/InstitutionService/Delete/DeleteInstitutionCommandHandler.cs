using MediatR;
using SharedKernel.Domain;
using ShellMgmt.Domain.InstitutionModels;

namespace ShellMgmt.Application.InstitutionService.Delete;

internal sealed class DeleteInstitutionCommandHandler(IRepository<Institution> repository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteInstitutionCommand, bool>
{
    public async Task<bool> Handle(DeleteInstitutionCommand command, CancellationToken cancellationToken)
    {
        var institution = await repository.GetById(command.Id);
        if (institution == null) return false;

        await repository.Delete(institution);
        await unitOfWork.SaveChanges(cancellationToken);

        return true;
    }
}
