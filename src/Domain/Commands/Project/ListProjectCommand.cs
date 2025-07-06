using Domain.Constants;

namespace Domain.Commands.Project;

public class ListProjectCommand : ICommand
{
    public string Command => BotCommands.ListMyProjects;
    
    public long? UserId { get; set; } 
}