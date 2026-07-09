using SharedKernel.Persistance.Abstractions;
using System.Linq.Expressions;
using ShellMgmt.Domain.OrgUnitModels;

namespace ShellMgmt.Application.OrgUnitService.OrgUnitSpecification;

public class OrgUnitGetSpec : Specification<OrgUnit>
{
    public OrgUnitGetSpec(OrgUnitDto dto, int take, int skip, string? sortBy, string? orderBy)
    {
        if (!string.IsNullOrEmpty(dto.Name))
        {
            ApplyCriteria(x => x.Name.Contains(dto.Name));
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

    private static Expression<Func<OrgUnit, object>> GetSortProperty(string? sortBy)
    {
        return sortBy?.ToLower() switch
        {
            "id" => x => x.Id,
            "name" => x => x.Name,
            _ => x => x.Id
        };
    }

    public override Expression<Func<OrgUnit, bool>> ToExpression()
    {
        throw new NotImplementedException();
    }
}
