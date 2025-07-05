using Domain.Commands;
using Domain.Commands.Project;

namespace Application.CommandHandlers.Project;

public class ProjectCommandHandler : ICommandHandler<ProjectCommand>
{
    public async Task<string?> Handle(ProjectCommand command)
    {
        
        return await Task.FromResult("Project");
        
    }
}