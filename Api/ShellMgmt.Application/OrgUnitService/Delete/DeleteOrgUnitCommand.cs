using MediatR;

namespace ShellMgmt.Application.OrgUnitService.Delete;

public sealed record DeleteOrgUnitCommand(Guid Id) : IRequest<bool>;
