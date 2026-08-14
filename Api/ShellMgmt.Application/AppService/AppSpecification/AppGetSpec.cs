using SharedKernel.Persistance.Abstractions;
using System.Linq.Expressions;
using ShellMgmt.Domain.AppModels;

namespace ShellMgmt.Application.AppService.AppSpecification;

public class AppGetSpec : Specification<App>
{
    public AppGetSpec(AppDto dto, int take, int skip, string? sortBy, string? orderBy)
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

    private static Expression<Func<App, object>> GetSortProperty(string? sortBy)
    {
        return sortBy?.ToLower() switch
        {
            "id" => x => x.Id,
            "name" => x => x.Name,
            "code" => x => x.Code,
            "createddate" => x => x.CreatedDate,
            _ => x => x.Id
        };
    }

    public override Expression<Func<App, bool>> ToExpression()
    {
        throw new NotImplementedException();
    }
}
