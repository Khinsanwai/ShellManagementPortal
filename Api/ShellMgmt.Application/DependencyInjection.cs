using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using ShellMgmt.Persistence.Ef.Repository;
using SharedKernel.Domain;

namespace ShellMgmt.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddService(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });

        services.AddTransient(typeof(IReadRepository<>), typeof(ReadRepository<>));
        services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
        services.AddTransient<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
