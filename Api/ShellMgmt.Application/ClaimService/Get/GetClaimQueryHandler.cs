using AutoMapper;
using MediatR;
using SharedKernel.Domain;
using ShellMgmt.Application.ClaimService.ClaimSpecification;
using ShellMgmt.Domain.ClaimModels;

namespace ShellMgmt.Application.ClaimService.Get;

internal sealed class GetClaimQueryHandler(IReadRepository<Claim> repository, IMapper mapper)
    : IRequestHandler<GetClaimQuery, PagedList<ClaimDto>>
{
    public async Task<PagedList<ClaimDto>> Handle(GetClaimQuery query, CancellationToken cancellationToken)
    {
        ClaimDto claimDto = mapper.Map<ClaimDto>(query);
        ClaimGetSpec spec = new(claimDto, query.Take, query.Skip, query.SortBy, query.OrderBy);
        var claims = await repository.Get(spec, cancellationToken);
        return mapper.Map<PagedList<ClaimDto>>(claims);
    }
}
