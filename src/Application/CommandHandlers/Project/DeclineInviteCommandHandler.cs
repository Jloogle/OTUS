using System.Text.RegularExpressions;
using Domain.Commands;
using Domain.Commands.Project;
using Domain.Services;

namespace Application.CommandHandlers.Project;

/// <summary>
/// Обрабатывает команду отклонения приглашения в проект.
/// </summary>
public class DeclineInviteCommandHandler(IInviteStore inviteStore) : ICommandHandler<DeclineInviteCommand>
{
    /// <summary>
    /// Обрабатывает команду отклонения приглашения в проект.
    /// </summary>
    public async Task<string?> Handle(DeclineInviteCommand command)
    {
        var id = ParseId(command.UserCommand);
        if (id is null) return "Используйте: /decline_invite [ID приглашения]";
        var inv = await inviteStore.GetAsync(id.Value);
        if (inv is null) return "Приглашение не найдено или истекло.";
        await inviteStore.RemoveAsync(id.Value);
        return "Приглашение отклонено.";
    }

    private static int? ParseId(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return null;
        var cleaned = Regex.Replace(input, @"^/decline_invite\\s*", "", RegexOptions.IgnoreCase).Trim();
        var m = Regex.Match(cleaned, @"\\[(.*?)\\]");
        if (!m.Success) return null;
        return int.TryParse(m.Groups[1].Value.Trim(), out var id) ? id : null;
    }
}
