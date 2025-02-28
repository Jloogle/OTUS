using Domain.Constants;

namespace Domain.Commands.Start;

public class StartCommand : ICommand
{
    public string Command => BotCommands.Start;
    
    public long? UserId { get; set; } 
}