using MediatR;
using ShellMgmt.Domain.InstitutionModels;

namespace ShellMgmt.Application.InstitutionService.Create;

public sealed record CreateInstitutionCommand(string Name, string? Code, string? Description) : IRequest<InstitutionDto>;
