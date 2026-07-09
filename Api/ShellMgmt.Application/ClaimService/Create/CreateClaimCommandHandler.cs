using AutoMapper;
using MediatR;
using SharedKernel.Domain;
using ShellMgmt.Domain.ClaimModels;

namespace ShellMgmt.Application.ClaimService.Create;

internal sealed class CreateClaimCommandHandler(IRepository<Claim> repository, IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<CreateClaimCommand, ClaimDto>
{
    public async Task<ClaimDto> Handle(CreateClaimCommand command, CancellationToken cancellationToken)
    {
        var claim = new Claim
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Value = command.Value,
            Description = command.Description
        };

        await repository.Add(claim);
        await unitOfWork.SaveChanges(cancellationToken);

        return mapper.Map<ClaimDto>(claim);
    }
}
