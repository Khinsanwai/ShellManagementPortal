using MediatR;
using ShellMgmt.Domain.InstitutionModels;

namespace ShellMgmt.Application.InstitutionService.Update;

public sealed record UpdateInstitutionCommand(Guid Id, string Name, string? Code, string? Description) : IRequest<InstitutionDto?>;
