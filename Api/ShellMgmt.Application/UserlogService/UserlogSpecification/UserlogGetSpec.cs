using SharedKernel.Persistance.Abstractions;
using System.Linq.Expressions;
using ShellMgmt.Domain.UserlogModels;

namespace ShellMgmt.Application.UserlogService.UserlogSpecification;

public class UserlogGetSpec : Specification<Userlog>
{
    public UserlogGetSpec(UserlogDto dto, int take, int skip, string? sortBy, string? orderBy,
        string? action = null, string? module = null, string? application = null,
        DateTime? dateFrom = null, DateTime? dateTo = null)
    {
        if (!string.IsNullOrEmpty(dto.Username))
        {
            ApplyCriteria(x => x.Username != null && x.Username.Contains(dto.Username));
        }

        if (!string.IsNullOrEmpty(dto.Category))
        {
            ApplyCriteria(x => x.Category.Contains(dto.Category));
        }

        if (!string.IsNullOrEmpty(dto.Result))
        {
            ApplyCriteria(x => x.Result.Contains(dto.Result));
        }

        if (!string.IsNullOrEmpty(action))
        {
            ApplyCriteria(x => x.Action.Contains(action));
        }

        if (!string.IsNullOrEmpty(module))
        {
            ApplyCriteria(x => x.Module != null && x.Module.Contains(module));
        }

        if (!string.IsNullOrEmpty(application))
        {
            ApplyCriteria(x => x.Application != null && x.Application.Contains(application));
        }

        if (dateFrom.HasValue)
        {
            ApplyCriteria(x => x.CreatedDate >= dateFrom.Value);
        }

        if (dateTo.HasValue)
        {
            // Include the entire end date
            var endDate = dateTo.Value.Date.AddDays(1);
            ApplyCriteria(x => x.CreatedDate < endDate);
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

    private static Expression<Func<Userlog, object>> GetSortProperty(string? sortBy)
    {
        return sortBy?.ToLower() switch
        {
            "id" => x => x.Id,
            "username" => x => x.Username ?? string.Empty,
            "category" => x => x.Category,
            "action" => x => x.Action,
            "module" => x => x.Module ?? string.Empty,
            "result" => x => x.Result,
            "createddate" => x => x.CreatedDate,
            _ => x => x.Id
        };
    }

    public override Expression<Func<Userlog, bool>> ToExpression()
    {
        throw new NotImplementedException();
    }
}
