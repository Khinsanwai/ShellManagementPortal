using MediatR;

namespace ShellMgmt.Application.InstitutionService.Delete;

public sealed record DeleteInstitutionCommand(Guid Id) : IRequest<bool>;
