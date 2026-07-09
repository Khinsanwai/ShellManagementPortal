using AutoMapper;
using MediatR;
using SharedKernel.Domain;
using ShellMgmt.Application.OrgUnitService.OrgUnitSpecification;
using ShellMgmt.Domain.OrgUnitModels;

namespace ShellMgmt.Application.OrgUnitService.Get;

internal sealed class GetOrgUnitQueryHandler(IReadRepository<OrgUnit> repository, IMapper mapper)
    : IRequestHandler<GetOrgUnitQuery, PagedList<OrgUnitDto>>
{
    public async Task<PagedList<OrgUnitDto>> Handle(GetOrgUnitQuery query, CancellationToken cancellationToken)
    {
        OrgUnitDto dto = mapper.Map<OrgUnitDto>(query);
        OrgUnitGetSpec spec = new(dto, query.Take, query.Skip, query.SortBy, query.OrderBy);
        var items = await repository.Get(spec, cancellationToken);
        return mapper.Map<PagedList<OrgUnitDto>>(items);
    }
}
