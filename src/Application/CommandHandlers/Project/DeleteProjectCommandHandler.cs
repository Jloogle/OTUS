using Domain.Commands;
using Domain.Commands.Project;

namespace Application.CommandHandlers.Project;

public class DeleteProjectCommandHandler : ICommandHandler<DeleteProjectCommand>
{
    public async Task<string?> Handle(DeleteProjectCommand command)
    {
        
        return await Task.FromResult("Удаление проекта - /project_delete [Id проекта].");
        
    }
}