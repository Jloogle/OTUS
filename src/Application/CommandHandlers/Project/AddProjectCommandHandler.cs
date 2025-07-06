using Domain.Commands;
using Domain.Commands.Project;

namespace Application.CommandHandlers.Project;

public class AddProjectCommandHandler : ICommandHandler<AddProjectCommand>
{
    public async Task<string?> Handle(AddProjectCommand command)
    {
        
        return await Task.FromResult("Создание проекта - /add_project [название задачи] [проект] [исполнитель] [дедлайн].");
        
    }
}