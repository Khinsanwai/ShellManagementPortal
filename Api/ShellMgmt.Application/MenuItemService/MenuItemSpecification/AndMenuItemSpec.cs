using SharedKernel.Domain;
using SharedKernel.Persistance.Abstractions;
using ShellMgmt.Domain.MenuItemModels;

namespace ShellMgmt.Application.MenuItemService.MenuItemSpecification;

public class AndMenuItemSpec : AndSpecification<MenuItem>
{
    public AndMenuItemSpec(params ISpecification<MenuItem>?[] specifications)
    {
        _Specifications = specifications ?? throw new ArgumentNullException(nameof(specifications));
    }
}
