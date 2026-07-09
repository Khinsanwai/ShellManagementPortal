using AutoMapper;
using MediatR;
using SharedKernel.Domain;
using ShellMgmt.Application.AppService.AppSpecification;
using ShellMgmt.Domain.AppModels;

namespace ShellMgmt.Application.AppService.Get;

internal sealed class GetAppQueryHandler(IReadRepository<App> repository, IMapper mapper)
    : IRequestHandler<GetAppQuery, PagedList<AppDto>>
{
    public async Task<PagedList<AppDto>> Handle(GetAppQuery query, CancellationToken cancellationToken)
    {
        AppDto dto = mapper.Map<AppDto>(query);
        AppGetSpec spec = new(dto, query.Take, query.Skip, query.SortBy, query.OrderBy);
        var items = await repository.Get(spec, cancellationToken);
        return mapper.Map<PagedList<AppDto>>(items);
    }
}
