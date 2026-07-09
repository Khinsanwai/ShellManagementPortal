using MediatR;
using ShellMgmt.Domain.OrgUnitModels;

namespace ShellMgmt.Application.OrgUnitService.Create;

public sealed record CreateOrgUnitCommand(string Name, Guid? ParentUnitId, string? Description) : IRequest<OrgUnitDto>;
