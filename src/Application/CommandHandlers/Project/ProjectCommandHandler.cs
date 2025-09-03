using System.Text;
using Domain.Commands;
using Domain.Commands.Project;
using Domain.Services;

namespace Application.CommandHandlers.Project;

public class ProjectCommandHandler(IInviteStore inviteStore) : ICommandHandler<ProjectCommand>
{
    public async Task<string?> Handle(ProjectCommand command)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Меню проектов:");
        sb.AppendLine("• Создать проект: /add_project");
        sb.AppendLine("• Удалить проект: /project_delete [ID]");

        if (command.UserId is not null)
        {
            var invites = await inviteStore.ListActiveForInviteeAsync(command.UserId.Value);
            if (invites.Any())
            {
                sb.AppendLine();
                sb.AppendLine($"У вас активных приглашений: {invites.Count()}");
                foreach (var inv in invites.Take(5))
                {
                    sb.AppendLine($"- Проект {inv.ProjectId}: принять /accept_invite [{inv.Id}], отклонить /decline_invite [{inv.Id}]");
                }
                if (invites.Count() > 5)
                    sb.AppendLine("… и другие. См. /invites");
                sb.AppendLine();
                sb.AppendLine("Кнопки 'Мои приглашения' и 'История приглашений' доступны в меню.");
            }
            else
            {
                sb.AppendLine();
                sb.AppendLine("У вас нет активных приглашений. Чтобы посмотреть, нажмите 'Мои приглашения'.");
            }
        }

        return sb.ToString();
    }
}
