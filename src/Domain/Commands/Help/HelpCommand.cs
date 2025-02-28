using Domain.Constants;

namespace Domain.Commands.Help;

public class HelpCommand : ICommand
{
    public string Command => BotCommands.Help;
    
    public long? UserId { get; set; } 
}