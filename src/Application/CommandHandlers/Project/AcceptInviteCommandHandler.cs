using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Domain.Commands;
using Domain.Commands.Project;
using Domain.Repositories;
using Domain.Services;

namespace Application.CommandHandlers.Project;

public class AcceptInviteCommandHandler(
    IInviteStore inviteStore,
    IProjectRepository projectRepository,
    IUserRepository userRepository,
    IRoleRepository roleRepository
) : ICommandHandler<AcceptInviteCommand>
{
    public async Task<string?> Handle(AcceptInviteCommand command)
    {
        var id = ParseId(command.UserCommand);
        if (id is null) return "Используйте: /accept_invite [ID приглашения]";

        var invite = await inviteStore.GetAsync(id.Value);
        if (invite is null) return "Приглашение не найдено или истекло.";

        // Проверяем совпадает ли принимающий с приглашаемым
        var acceptor = await userRepository.FindByIdTelegram(command.UserId);
        if (acceptor is null || acceptor.IdTelegram != invite.InviteeTelegramId)
            return "Это приглашение не для вас.";

        await projectRepository.AddUserToProjectAsync(invite.ProjectId, acceptor.Id);

        if (!string.IsNullOrWhiteSpace(invite.RoleName))
        {
            var role = await roleRepository.GetByNameAsync(invite.RoleName);
            if (role != null)
            {
                await userRepository.AddRoleToUserAsync(acceptor.Id, role.Id);
            }
        }

        await inviteStore.RemoveAsync(invite.Id);
        return $"Вы добавлены в проект {invite.ProjectId}.";
    }

    private static int? ParseId(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return null;
        var val = input.Split(' ');
        if (val.Length != 2) return null;
        if (val[0] != "/accept_invite") return null;
        if (string.IsNullOrWhiteSpace(val[1])) return null;
        var cleaned = int.TryParse(val[1].Replace("[", "").Replace("]", ""), out var id) ? id : (int?)null;
        return cleaned;
    }
}

