using Domain.Commands;
using Domain.Constants;

namespace Domain.Commands.Project;

public class InviteHistoryCommand : ICommand
{
    public long? UserId { get; set; }
    public string Command => BotCommands.InviteHistory;
    public string? UserCommand { get; set; }
}

