using SharedKernel.Persistance.Abstractions;
using System.Linq.Expressions;
using ShellMgmt.Domain.AppModels;

namespace ShellMgmt.Application.AppService.AppSpecification;

public class AppByIdSpec : Specification<App>
{
    public AppByIdSpec(int id)
    {
        ApplyCriteria(x => x.Id == id);
    }

    public override Expression<Func<App, bool>> ToExpression()
    {
        throw new NotImplementedException();
    }
}
