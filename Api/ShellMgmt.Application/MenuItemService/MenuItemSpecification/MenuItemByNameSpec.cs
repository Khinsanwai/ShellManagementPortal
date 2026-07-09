using SharedKernel.Persistance.Abstractions;
using System.Linq.Expressions;
using ShellMgmt.Domain.MenuItemModels;

namespace ShellMgmt.Application.MenuItemService.MenuItemSpecification;

public class MenuItemByNameSpec(string name) : Specification<MenuItem>(x => x.Name.Contains(name))
{
    public override Expression<Func<MenuItem, bool>> ToExpression()
    {
        return menuItem => menuItem.Name.Contains(name);
    }
}
