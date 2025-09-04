using System.Text.RegularExpressions;
using Domain.Commands;
using Domain.Commands.Project;
using Domain.Repositories;
using Domain.Services;
using Telegram.Bot;

namespace Application.CommandHandlers.Project;

/// <summary>
/// Обрабатывает приглашение пользователя в проект и отправляет действия Telegram для принятия/отклонения.
/// </summary>
public class InviteProjectMemberCommandHandler(
    IProjectRepository projectRepository,
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IInviteStore inviteStore,
    ITelegramBotClient bot
) : ICommandHandler<InviteProjectMemberCommand>
{
    /// <summary>
    /// Разбирает ввод, создаёт приглашение в хранилище и уведомляет приглашённого.
    /// </summary>
    public async Task<string?> Handle(InviteProjectMemberCommand command)
    {
        var parts = Parse(command.UserCommand);
        if (parts.Length < 2)
            return "Приглашение: /project_invite [ID проекта] [email] [роль?]";

        if (!int.TryParse(parts[0], out var projectId))
            return "Некорректный ID проекта.";

        var email = parts[1];
        string? roleName = parts.Length >= 3 ? parts[2] : null;

        var user = await userRepository.FindByEmailAsync(email);
        if (user == null)
            return $"Пользователь с email {email} не найден.";

        // Создаем приглашение в Redis и отправляем уведомление
        var inviteId = await inviteStore.CreateAsync(projectId, command.UserId ?? 0, user.IdTelegram ?? 0, roleName);
        var text = $"Вас пригласили в проект {projectId}.";
        if (user.IdTelegram is not null && user.IdTelegram > 0)
        {
            var keyboard = new Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardMarkup
            {
                Keyboard =
                [
                    [new Telegram.Bot.Types.ReplyMarkups.KeyboardButton($"/accept_invite [{inviteId}]")],
                    [new Telegram.Bot.Types.ReplyMarkups.KeyboardButton($"/decline_invite [{inviteId}]")]
                ],
                ResizeKeyboard = true
            };
            await bot.SendMessage(user.IdTelegram.Value, text, replyMarkup: keyboard);
        }

        return $"Приглашение отправлено пользователю {user.Email}. ID приглашения: {inviteId}.";
    }

    /// <summary>
    /// Разбор текста команды: извлекает параметры внутри квадратных скобок.
    /// </summary>
    private static string[] Parse(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return [];
        var cleaned = Regex.Replace(input, @"^/project_invite\s*", "", RegexOptions.IgnoreCase).Trim();
        var matches = Regex.Matches(cleaned, @"\[(.*?)\]");
        return matches.Select(m => m.Groups[1].Value.Trim()).ToArray();
    }
}
