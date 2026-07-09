using AutoMapper;
using MediatR;
using SharedKernel.Domain;
using ShellMgmt.Application.ResourceService.ResourceSpecification;
using ShellMgmt.Domain.ResourceModels;

namespace ShellMgmt.Application.ResourceService.Get;

internal sealed class GetResourceQueryHandler(IReadRepository<Resource> repository, IMapper mapper)
    : IRequestHandler<GetResourceQuery, PagedList<ResourceDto>>
{
    public async Task<PagedList<ResourceDto>> Handle(GetResourceQuery query, CancellationToken cancellationToken)
    {
        ResourceDto resourceDto = mapper.Map<ResourceDto>(query);
        ResourceGetSpec spec = new(resourceDto, query.Take, query.Skip, query.SortBy, query.OrderBy);
        var resources = await repository.Get(spec, cancellationToken);
        return mapper.Map<PagedList<ResourceDto>>(resources);
    }
}
