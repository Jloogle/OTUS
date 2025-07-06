using Domain.Commands;
using Domain.Commands.Project;

namespace Application.CommandHandlers.Project;

public class ListProjectCommandHandler : ICommandHandler<ListProjectCommand>
{
    public async Task<string?> Handle(ListProjectCommand command)
    {
        
        return await Task.FromResult("ListProject");
        
    }
}