using Domain.Constants;

namespace Domain.Commands.Project;

/// <summary>
/// Команда для принятия приглашения в проект.
/// </summary>
public class AcceptInviteCommand : ICommand
{
    public string Command => BotCommands.AcceptInvite;
    public string? UserCommand { get; set; }
    public long? UserId { get; set; }
}
