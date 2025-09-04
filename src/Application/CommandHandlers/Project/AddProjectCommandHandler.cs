using Domain.Commands;
using Domain.Commands.Project;
using Domain.Repositories;
using System.Text.Json;
using Domain.Services;
using Telegram.Bot;
using System.Text.RegularExpressions;

namespace Application.CommandHandlers.Project;

/// <summary>
/// Обрабатывает команду создания проекта и проводит пошаговую процедуру создания.
/// </summary>
public class AddProjectCommandHandler(
    IProjectRepository projectRepository,
    IRadisRepository redis,
    IUserRepository userRepository,
    IInviteStore inviteStore,
    ITelegramBotClient bot
) : ICommandHandler<AddProjectCommand>
{
    /// <summary>
    /// Обрабатывает команду создания проекта и проводит пошаговую процедуру создания.
    /// </summary>
    public async Task<string?> Handle(AddProjectCommand command)
    {
        var key = $"ProjCreate:{command.UserId}";
        var text = command.UserCommand ?? string.Empty;
        var isJustCommand = string.Equals(text, command.Command, StringComparison.OrdinalIgnoreCase);

        var draft = GetDraft(key);

        if (isJustCommand || draft.State == 0)
        {
            draft.State = 1; // Name
            SaveDraft(key, draft);
            return "Создание проекта. Введите название проекта:";
        }

        switch (draft.State)
        {
            case 1: // Name
                draft.Name = text.Trim();
                draft.State = 2; // Emails
                SaveDraft(key, draft);
                return "Укажите email-адреса участников через запятую, либо '-' если пока никого не приглашать:";
            case 2: // Emails
                if (text.Trim() != "-")
                {
                    draft.Emails = text
                        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .ToList();
                }
                draft.State = 3; // Deadline
                SaveDraft(key, draft);
                return "Укажите дедлайн в формате ГГГГ-ММ-ДД, либо '-' если без дедлайна:";
            case 3: // Deadline
                if (text.Trim() != "-")
                {
                    var deadline = ConvertToDateTime(text);
                    if (!deadline.HasValue)
                    {
                        return "Неверный формат даты. Укажите дедлайн в формате ГГГГ-ММ-ДД, либо '-' если без дедлайна:";
                    }
                    draft.Deadline = deadline.Value;
                }

                // Create project
                var dl = draft.Deadline ?? DateTime.UtcNow.AddDays(30);
                var projectId = await projectRepository.AddProjectAsync(draft.Name!, dl);

                // Add creator as project member
                if (command.UserId is not null)
                {
                    var creator = await userRepository.FindByIdTelegram(command.UserId.Value);
                    if (creator != null)
                    {
                        try
                        {
                            await projectRepository.AddUserToProjectAsync(projectId, creator.Id);
                        }
                        catch (Exception ex)
                        {
                            // Логируем ошибку, но не прерываем выполнение
                            Console.WriteLine($"Ошибка добавления пользователя в проект: {ex.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Пользователь с Telegram ID {command.UserId.Value} не найден");
                    }
                }

                // Create invites by email if possible
                if (draft.Emails?.Count > 0 && command.UserId is not null)
                {
                    foreach (var email in draft.Emails)
                    {
                        var u = await userRepository.FindByEmailAsync(email);
                        if (u?.IdTelegram is not null && u.IdTelegram > 0)
                        {
                            var inviteId = await inviteStore.CreateAsync(projectId, command.UserId.Value, u.IdTelegram.Value, null);
                            var textNotify = $"Вас пригласили в проект {projectId}.";
                            var keyboard = new Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardMarkup
                            {
                                Keyboard =
                                [
                                    [new Telegram.Bot.Types.ReplyMarkups.KeyboardButton($"/accept_invite [{inviteId}]")],
                                    [new Telegram.Bot.Types.ReplyMarkups.KeyboardButton($"/decline_invite [{inviteId}]")]
                                ],
                                ResizeKeyboard = true
                            };
                            await bot.SendMessage(u.IdTelegram.Value, textNotify, replyMarkup: keyboard);
                        }
                    }
                }

                // Clear draft
                DeleteDraft(key);
                return $"Проект '{draft.Name}' создан (ID: {projectId}). Для приглашения используйте /project_invite [{projectId}] [email] [роль?].";
            default:
                draft.State = 1; SaveDraft(key, draft);
                return "Создание проекта. Введите название проекта:";
        }
    }

    private static DateTime? ConvertToDateTime(string value)
    {
        DateTime convertedDate;
        try {
            convertedDate = Convert.ToDateTime(value);
            return convertedDate;
        }
        catch (FormatException) {
            return null;
        }
    }

    private string[] ParseAddProjectCommand(string command)
    {
        // Remove the "/add_project" part and trim any leading/trailing whitespace
        var cleanedCommand = Regex.Replace(command, @"^/add_project\s*", "", RegexOptions.IgnoreCase).Trim();

        // If no square brackets, split by whitespace
        if (!cleanedCommand.Contains('['))
        {
            return cleanedCommand.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }

        // Match content within square brackets
        var matches = Regex.Matches(cleanedCommand, @"\[(.*?)\]");

        // Extract matched content and return as array
        return matches.Select(m => m.Groups[1].Value.Trim()).ToArray();
    }

    private record ProjectDraft
    {
        public int State { get; set; }
        public string? Name { get; set; }
        public List<string>? Emails { get; set; }
        public DateTime? Deadline { get; set; }
    }

    private ProjectDraft GetDraft(string key)
    {
        var json = redis.StringGet(key);
        if (string.IsNullOrEmpty(json)) return new ProjectDraft();
        return JsonSerializer.Deserialize<ProjectDraft>(json) ?? new ProjectDraft();
    }

    private void SaveDraft(string key, ProjectDraft draft)
    {
        redis.StringSet(key, JsonSerializer.Serialize(draft));
    }

    private void DeleteDraft(string key)
    {
        redis.StringDelete(key);
    }
}
