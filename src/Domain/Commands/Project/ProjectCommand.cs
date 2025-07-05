using Domain.Constants;

namespace Domain.Commands.Project;

public class ProjectCommand : ICommand
{
    public string Command => BotCommands.Project;
    
    public long? UserId { get; set; } 
}