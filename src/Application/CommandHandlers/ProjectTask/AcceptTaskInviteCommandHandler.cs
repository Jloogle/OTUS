using System.Text.RegularExpressions;
using Domain.Commands;
using Domain.Commands.Task;
using Domain.Repositories;
using Domain.Services;

namespace Application.CommandHandlers.ProjectTask;

/// <summary>
/// Принимает приглашение на назначение задачи и выполняет назначение.
/// </summary>
public class AcceptTaskInviteCommandHandler(ITaskInviteStore inviteStore, IUserRepository userRepository, ITaskRepository taskRepository) : ICommandHandler<AcceptTaskInviteCommand>
{
    public async Task<string?> Handle(AcceptTaskInviteCommand command)
    {
        var id = ParseId(command.UserCommand);
        if (id is null) return "Используйте: /accept_task [ID]";
        var invite = await inviteStore.GetAsync(id.Value);
        if (invite is null) return "Приглашение на задачу не найдено или истекло.";

        var user = await userRepository.FindByIdTelegram(command.UserId);
        if (user is null || user.IdTelegram != invite.AssigneeTelegramId)
            return "Это приглашение на задачу не для вас.";

        await taskRepository.AssignTaskToUserAsync(invite.TaskId, user.Id);
        await inviteStore.RemoveAsync(invite.Id);
        return $"Задача {invite.TaskId} назначена вам.";
    }

    private static int? ParseId(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return null;
        var cleaned = Regex.Replace(input, @"^/accept_task\s*", "", RegexOptions.IgnoreCase).Trim();
        var m = Regex.Match(cleaned, @"\[(.*?)\]");
        if (!m.Success) return null;
        return int.TryParse(m.Groups[1].Value.Trim(), out var id) ? id : null;
    }
}

