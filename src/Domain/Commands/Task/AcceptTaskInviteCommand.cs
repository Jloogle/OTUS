using Domain.Constants;

namespace Domain.Commands.Task;

public class AcceptTaskInviteCommand : ICommand
{
    public string Command => BotCommands.AcceptTask;
    public string? UserCommand { get; set; }
    public long? UserId { get; set; }
}

