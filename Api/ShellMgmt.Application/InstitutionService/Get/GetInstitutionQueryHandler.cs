using AutoMapper;
using MediatR;
using SharedKernel.Domain;
using ShellMgmt.Application.InstitutionService.InstitutionSpecification;
using ShellMgmt.Domain.InstitutionModels;

namespace ShellMgmt.Application.InstitutionService.Get;

internal sealed class GetInstitutionQueryHandler(IReadRepository<Institution> repository, IMapper mapper)
    : IRequestHandler<GetInstitutionQuery, PagedList<InstitutionDto>>
{
    public async Task<PagedList<InstitutionDto>> Handle(GetInstitutionQuery query, CancellationToken cancellationToken)
    {
        InstitutionDto dto = mapper.Map<InstitutionDto>(query);
        InstitutionGetSpec spec = new(dto, query.Take, query.Skip, query.SortBy, query.OrderBy);
        var items = await repository.Get(spec, cancellationToken);
        return mapper.Map<PagedList<InstitutionDto>>(items);
    }
}
