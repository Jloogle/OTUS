using Domain.Constants;

namespace Domain.Commands.Project;

public class AddProjectCommand : ICommand
{
    public string Command => BotCommands.AddProject;
    
    public long? UserId { get; set; } 
}