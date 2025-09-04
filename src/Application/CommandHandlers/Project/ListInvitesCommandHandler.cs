using System.Text;
using Domain.Commands;
using Domain.Commands.Project;
using Domain.Services;

namespace Application.CommandHandlers.Project;

/// <summary>
/// Показывает активные приглашения для текущего пользователя.
/// </summary>
public class ListInvitesCommandHandler(IInviteStore inviteStore) : ICommandHandler<ListInvitesCommand>
{
    /// <summary>
    /// Получает приглашения и формирует действия для принятия/отклонения.
    /// </summary>
    public async Task<string?> Handle(ListInvitesCommand command)
    {
        if (command.UserId is null)
            return "Не удалось определить пользователя.";

        var invites = await inviteStore.ListActiveForInviteeAsync(command.UserId.Value);
        if (!invites.Any())
            return "У вас нет активных приглашений.";

        var sb = new StringBuilder();
        sb.AppendLine("Ваши активные приглашения:");
        foreach (var inv in invites)
        {
            sb.AppendLine($"• Проект {inv.ProjectId} — принять: /accept_invite [{inv.Id}], отклонить: /decline_invite [{inv.Id}]");
        }

        return sb.ToString();
    }
}
