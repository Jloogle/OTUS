using Domain.Constants;

namespace Domain.Commands.Task;

public class DeclineTaskInviteCommand : ICommand
{
    public string Command => BotCommands.DeclineTask;
    public string? UserCommand { get; set; }
    public long? UserId { get; set; }
}

