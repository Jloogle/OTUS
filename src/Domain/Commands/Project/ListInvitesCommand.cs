using Domain.Commands;
using Domain.Constants;

namespace Domain.Commands.Project;

/// <summary>
/// Команда для просмотра списка входящих приглашений.
/// </summary>
public class ListInvitesCommand : ICommand
{
    public long? UserId { get; set; }
    public string Command => BotCommands.ListInvites;
    public string? UserCommand { get; set; }
}
