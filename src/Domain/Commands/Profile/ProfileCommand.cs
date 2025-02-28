using Domain.Constants;

namespace Domain.Commands.Profile;

public class ProfileCommand : ICommand
{
    public string Command => BotCommands.Profile;
    
    public long? UserId { get; set; }
}