using SharedKernel.Persistance.Abstractions;
using System.Linq.Expressions;
using ShellMgmt.Domain.ClaimModels;

namespace ShellMgmt.Application.ClaimService.ClaimSpecification;

public class ClaimGetSpec : Specification<Claim>
{
    public ClaimGetSpec(ClaimDto claimDto, int take, int skip, string? sortBy, string? orderBy)
    {
        if (!string.IsNullOrEmpty(claimDto.Name))
        {
            ApplyCriteria(x => x.Name.Contains(claimDto.Name));
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

    private static Expression<Func<Claim, object>> GetSortProperty(string? sortBy)
    {
        return sortBy?.ToLower() switch
        {
            "id" => x => x.Id,
            "name" => x => x.Name,
            _ => x => x.Id
        };
    }

    public override Expression<Func<Claim, bool>> ToExpression()
    {
        throw new NotImplementedException();
    }
}
