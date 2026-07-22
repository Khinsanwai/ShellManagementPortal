using AutoMapper;
using MediatR;
using SharedKernel.Domain;
using ShellMgmt.Application.UserlogService.UserlogSpecification;
using ShellMgmt.Domain.UserlogModels;

namespace ShellMgmt.Application.UserlogService.Get;

internal sealed class GetUserlogQueryHandler(IReadRepository<Userlog> repository, IMapper mapper)
    : IRequestHandler<GetUserlogQuery, PagedList<UserlogDto>>
{
    public async Task<PagedList<UserlogDto>> Handle(GetUserlogQuery query, CancellationToken cancellationToken)
    {
        UserlogDto dto = mapper.Map<UserlogDto>(query);
        UserlogGetSpec spec = new(dto, query.Take, query.Skip, query.SortBy, query.OrderBy,
            action: query.Action, module: query.Module, application: query.Application,
            dateFrom: query.DateFrom, dateTo: query.DateTo);
        var items = await repository.Get(spec, cancellationToken);
        return mapper.Map<PagedList<UserlogDto>>(items);
    }
}
