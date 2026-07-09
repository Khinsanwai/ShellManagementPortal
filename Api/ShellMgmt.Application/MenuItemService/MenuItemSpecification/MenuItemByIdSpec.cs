using SharedKernel.Persistance.Abstractions;
using System.Linq.Expressions;
using ShellMgmt.Domain.MenuItemModels;

namespace ShellMgmt.Application.MenuItemService.MenuItemSpecification;

public class MenuItemByIdSpec(Guid id) : Specification<MenuItem>(x => x.Id == id)
{
    public override Expression<Func<MenuItem, bool>> ToExpression()
    {
        return menuItem => menuItem.Id == id;
    }
}
