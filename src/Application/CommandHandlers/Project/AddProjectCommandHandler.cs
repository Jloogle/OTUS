using Domain.Commands;
using Domain.Commands.Project;
using Domain.Repositories;

namespace Application.CommandHandlers.Project;

public class AddProjectCommandHandler(IProjectRepository projectRepository) : ICommandHandler<AddProjectCommand>
{
    public async Task<string?> Handle(AddProjectCommand command)
    {
        command.Command;
        
        return await Task.FromResult("Создание проекта - /add_project [название задачи] [дедлайн].");
        
    }
}