using AutoMapper;
using MediatR;
using SharedKernel.Domain;
using ShellMgmt.Domain.AppModels;

namespace ShellMgmt.Application.AppService.Create;

internal sealed class CreateAppCommandHandler(IRepository<App> repository, IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<CreateAppCommand, AppDto>
{
    public async Task<AppDto> Handle(CreateAppCommand command, CancellationToken cancellationToken)
    {
        var entity = new App
        {
            Name = command.Name,
            Code = command.Code,
            URL = command.URL,
            Icon = command.Icon,
            Description = command.Description,
            Version = command.Version,
            Status = command.Status,
            CreatedDate = DateTime.UtcNow
        };

        await repository.Add(entity);
        await unitOfWork.SaveChanges(cancellationToken);

        return mapper.Map<AppDto>(entity);
    }
}
