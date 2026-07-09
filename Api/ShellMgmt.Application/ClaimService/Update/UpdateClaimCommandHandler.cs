using AutoMapper;
using MediatR;
using SharedKernel.Domain;
using ShellMgmt.Domain.ClaimModels;

namespace ShellMgmt.Application.ClaimService.Update;

internal sealed class UpdateClaimCommandHandler(IRepository<Claim> repository, IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<UpdateClaimCommand, ClaimDto?>
{
    public async Task<ClaimDto?> Handle(UpdateClaimCommand command, CancellationToken cancellationToken)
    {
        var claim = await repository.GetById(command.Id);
        if (claim == null) return null;

        claim.Name = command.Name;
        claim.Value = command.Value;
        claim.Description = command.Description;

        await repository.Update(claim);
        await unitOfWork.SaveChanges(cancellationToken);

        return mapper.Map<ClaimDto>(claim);
    }
}
