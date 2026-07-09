using AutoMapper;
using MediatR;
using SharedKernel.Domain;
using ShellMgmt.Application.TenantService.TenantSpecification;
using ShellMgmt.Domain.TenantModels;

namespace ShellMgmt.Application.TenantService.GetByName;

public sealed class GetTenantByNameQueryHandler(IReadRepository<Tenant> repository, IMapper mapper)
    : IRequestHandler<GetTenantByNameQuery, TenantDto?>
{
    public async Task<TenantDto?> Handle(GetTenantByNameQuery query, CancellationToken cancellationToken)
    {
        TenantByNameSpec spec = new(query.Name);
        var tenant = await repository.Get(spec, cancellationToken);
        var tenantDtos = mapper.Map<List<TenantDto>>(tenant.Items);
        return tenantDtos.FirstOrDefault();
    }
}
