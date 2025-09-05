using System.Text.RegularExpressions;
using Domain.Commands;
using Domain.Commands.Task;
using Domain.Services;

namespace Application.CommandHandlers.ProjectTask;

/// <summary>
/// Отклоняет приглашение на назначение задачи.
/// </summary>
public class DeclineTaskInviteCommandHandler(ITaskInviteStore inviteStore) : ICommandHandler<DeclineTaskInviteCommand>
{
    public async Task<string?> Handle(DeclineTaskInviteCommand command)
    {
        var id = ParseId(command.UserCommand);
        if (id is null) return "Используйте: /decline_task [ID]";
        await inviteStore.RemoveAsync(id.Value);
        return "Приглашение на задачу отклонено.";
    }

    private static int? ParseId(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return null;
        var cleaned = Regex.Replace(input, @"^/decline_task\s*", "", RegexOptions.IgnoreCase).Trim();
        var m = Regex.Match(cleaned, @"\[(.*?)\]");
        if (!m.Success) return null;
        return int.TryParse(m.Groups[1].Value.Trim(), out var id) ? id : null;
    }
}

