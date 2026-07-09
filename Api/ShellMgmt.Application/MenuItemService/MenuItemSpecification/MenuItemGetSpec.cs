using SharedKernel.Persistance.Abstractions;
using System.Linq.Expressions;
using ShellMgmt.Domain.MenuItemModels;

namespace ShellMgmt.Application.MenuItemService.MenuItemSpecification;

public class MenuItemGetSpec : Specification<MenuItem>
{
    public MenuItemGetSpec(MenuItemDto menuItemDto, int take, int skip, string? sortBy, string? orderBy)
    {
        var byName = !string.IsNullOrEmpty(menuItemDto.Name) ? new MenuItemByNameSpec(menuItemDto.Name) : null;
        var byId = menuItemDto.Id.HasValue ? new MenuItemByIdSpec(menuItemDto.Id.Value) : null;

        var andSpec = new AndMenuItemSpec(byName, byId);
        var expression = andSpec.ToExpression();

        if (expression != null)
        {
            ApplyCriteria(expression);
        }

        ApplyPaging(skip, take);

        var orderByExpression = GetSortProperty(sortBy);

        if (orderBy != null && orderBy.Equals("ASC", StringComparison.CurrentCultureIgnoreCase))
        {
            ApplyOrderBy(orderByExpression);
        }
        else
        {
            ApplyOrderByDescending(orderByExpression);
        }
    }

    private static Expression<Func<MenuItem, object>> GetSortProperty(string? sortBy)
    {
        return sortBy?.ToLower() switch
        {
            "id" => x => x.Id,
            "name" => x => x.Name,
            _ => x => x.Id
        };
    }

    public override Expression<Func<MenuItem, bool>> ToExpression()
    {
        throw new NotImplementedException();
    }
}
