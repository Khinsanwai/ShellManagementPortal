using AutoMapper;
using MediatR;
using SharedKernel.Domain;
using ShellMgmt.Domain.UserlogModels;

namespace ShellMgmt.Application.UserlogService.Create;

internal sealed class CreateUserlogCommandHandler(IRepository<Userlog> repository, IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<CreateUserlogCommand, UserlogDto>
{
    public async Task<UserlogDto> Handle(CreateUserlogCommand command, CancellationToken cancellationToken)
    {
        var entity = new Userlog
        {
            UserId = command.UserId,
            Username = command.Username,
            Category = command.Category,
            Action = command.Action,
            Application = command.Application,
            Module = command.Module,
            Description = command.Description,
            Result = command.Result,
            IPAddress = command.IPAddress,
            UserAgent = command.UserAgent,
            CreatedDate = DateTime.UtcNow
        };

        await repository.Add(entity);
        await unitOfWork.SaveChanges(cancellationToken);

        return mapper.Map<UserlogDto>(entity);
    }
}
