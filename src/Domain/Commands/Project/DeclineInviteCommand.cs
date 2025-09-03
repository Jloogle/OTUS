using Domain.Constants;

namespace Domain.Commands.Project;

public class DeclineInviteCommand : ICommand
{
    public string Command => BotCommands.DeclineInvite;
    public string? UserCommand { get; set; }
    public long? UserId { get; set; }
}

