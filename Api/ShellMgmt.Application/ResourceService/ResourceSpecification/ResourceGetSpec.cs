using SharedKernel.Persistance.Abstractions;
using System.Linq.Expressions;
using ShellMgmt.Domain.ResourceModels;

namespace ShellMgmt.Application.ResourceService.ResourceSpecification;

public class ResourceGetSpec : Specification<Resource>
{
    public ResourceGetSpec(ResourceDto resourceDto, int take, int skip, string? sortBy, string? orderBy)
    {
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

    private static Expression<Func<Resource, object>> GetSortProperty(string? sortBy)
    {
        return sortBy?.ToLower() switch
        {
            "id" => x => x.Id,
            "name" => x => x.Name,
            _ => x => x.Id
        };
    }

    public override Expression<Func<Resource, bool>> ToExpression()
    {
        throw new NotImplementedException();
    }
}
