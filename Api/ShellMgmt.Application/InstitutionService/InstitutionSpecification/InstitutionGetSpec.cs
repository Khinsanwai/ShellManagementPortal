using SharedKernel.Persistance.Abstractions;
using System.Linq.Expressions;
using ShellMgmt.Domain.InstitutionModels;

namespace ShellMgmt.Application.InstitutionService.InstitutionSpecification;

public class InstitutionGetSpec : Specification<Institution>
{
    public InstitutionGetSpec(InstitutionDto dto, int take, int skip, string? sortBy, string? orderBy)
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

    private static Expression<Func<Institution, object>> GetSortProperty(string? sortBy)
    {
        return sortBy?.ToLower() switch
        {
            "id" => x => x.Id,
            "name" => x => x.Name,
            _ => x => x.Id
        };
    }

    public override Expression<Func<Institution, bool>> ToExpression()
    {
        throw new NotImplementedException();
    }
}
