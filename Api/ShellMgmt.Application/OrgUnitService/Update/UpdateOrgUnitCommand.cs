using MediatR;
using ShellMgmt.Domain.OrgUnitModels;

namespace ShellMgmt.Application.OrgUnitService.Update;

public sealed record UpdateOrgUnitCommand(Guid Id, string Name, Guid? ParentUnitId, string? Description) : IRequest<OrgUnitDto?>;
