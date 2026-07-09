using SharedKernel.Persistance.Abstractions;
using System.Linq.Expressions;
using ShellMgmt.Domain.TenantModels;

namespace ShellMgmt.Application.TenantService.TenantSpecification;

public class TenantByNameSpec(string name) : Specification<Tenant>(x => x.Name.Contains(name))
{
    public override Expression<Func<Tenant, bool>> ToExpression()
    {
        return tenant => tenant.Name.Contains(name);
    }
}
