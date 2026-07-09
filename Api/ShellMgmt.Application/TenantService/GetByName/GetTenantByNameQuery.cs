using MediatR;
using ShellMgmt.Domain.TenantModels;

namespace ShellMgmt.Application.TenantService.GetByName;

public sealed record GetTenantByNameQuery(string Name) : IRequest<TenantDto?>;
