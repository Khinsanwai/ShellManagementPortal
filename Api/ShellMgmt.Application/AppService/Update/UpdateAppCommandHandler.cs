using AutoMapper;
using MediatR;
using SharedKernel.Domain;
using ShellMgmt.Application.AppService.AppSpecification;
using ShellMgmt.Domain.AppModels;

namespace ShellMgmt.Application.AppService.Update;

internal sealed class UpdateAppCommandHandler(IRepository<App> repository, IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<UpdateAppCommand, AppDto?>
{
    public async Task<AppDto?> Handle(UpdateAppCommand command, CancellationToken cancellationToken)
    {
        var spec = new AppByIdSpec(command.Id);
        var entities = await repository.Get(spec, cancellationToken);
        var entity = entities.Items.FirstOrDefault();
        if (entity == null) return null;

        entity.Name = command.Name;
        entity.Code = command.Code;
        entity.URL = command.URL;
        entity.Icon = command.Icon;
        entity.Description = command.Description;
        entity.Version = command.Version;
        entity.Status = command.Status;
        entity.UpdatedDate = DateTime.UtcNow;

        await repository.Update(entity);
        await unitOfWork.SaveChanges(cancellationToken);

        return mapper.Map<AppDto>(entity);
    }
}
