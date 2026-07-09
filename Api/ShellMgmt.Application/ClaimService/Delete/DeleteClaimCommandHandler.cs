using MediatR;
using SharedKernel.Domain;
using ShellMgmt.Domain.ClaimModels;

namespace ShellMgmt.Application.ClaimService.Delete;

internal sealed class DeleteClaimCommandHandler(IRepository<Claim> repository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteClaimCommand, bool>
{
    public async Task<bool> Handle(DeleteClaimCommand command, CancellationToken cancellationToken)
    {
        var claim = await repository.GetById(command.Id);
        if (claim == null) return false;

        await repository.Delete(claim);
        await unitOfWork.SaveChanges(cancellationToken);

        return true;
    }
}
