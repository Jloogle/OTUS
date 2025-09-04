using Domain.Constants;

namespace Domain.Commands.Project;

/// <summary>
/// Команда для приглашения участника в проект.
/// </summary>
public class InviteProjectMemberCommand : ICommand
{
    public string Command => BotCommands.ProjectInvite;
    public string? UserCommand { get; set; }
    public long? UserId { get; set; }
}
