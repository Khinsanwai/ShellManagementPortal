using AutoMapper;
using MediatR;
using SharedKernel.Domain;
using ShellMgmt.Application.MenuItemService.MenuItemSpecification;
using ShellMgmt.Domain.MenuItemModels;

namespace ShellMgmt.Application.MenuItemService.Get;

internal sealed class GetMenuItemQueryHandler(IReadRepository<MenuItem> repository, IMapper mapper)
    : IRequestHandler<GetMenuItemQuery, PagedList<MenuItemDto>>
{
    public async Task<PagedList<MenuItemDto>> Handle(GetMenuItemQuery query, CancellationToken cancellationToken)
    {
        MenuItemDto menuItemDto = mapper.Map<MenuItemDto>(query);
        MenuItemGetSpec spec = new(menuItemDto, query.Take, query.Skip, query.SortBy, query.OrderBy);
        var menuItems = await repository.Get(spec, cancellationToken);
        return mapper.Map<PagedList<MenuItemDto>>(menuItems);
    }
}
