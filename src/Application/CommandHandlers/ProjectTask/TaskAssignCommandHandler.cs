using System.Text.RegularExpressions;
using Domain.Commands;
using Domain.Commands.Task;
using Domain.Repositories;
using Domain.Services;
using Telegram.Bot;

namespace Application.CommandHandlers.ProjectTask;

/// <summary>
/// Создаёт инвайт на назначение задачи пользователю проекта по email (с подтверждением получателем).
/// </summary>
public class TaskAssignCommandHandler(ITaskRepository taskRepository, IUserRepository userRepository, ITaskInviteStore taskInviteStore, ITelegramBotClient bot, IProjectRepository projectRepository) : ICommandHandler<TaskAssignCommand>
{
    public async Task<string?> Handle(TaskAssignCommand command)
    {
        var parts = Parse(command.UserCommand);
        if (parts.Length != 2) return "Назначение задачи: /task_assign [ID задачи] [email]";
        if (!int.TryParse(parts[0], out var taskId)) return "Некорректный ID задачи.";
        var email = parts[1];
        var user = await userRepository.FindByEmailAsync(email);
        if (user is null) return $"Пользователь с email {email} не найден.";
        if (user.IdTelegram is null) return "У пользователя нет Telegram ID для отправки приглашения.";

        var task = await taskRepository.GetTaskWithProjectAsync(taskId);
        if (task is null || task.Project is null)
        {
            return "Задача не найдена или не привязана к проекту.";
        }

        var members = await projectRepository.GetProjectMembersAsync(task.Project.Id);
        if (!members.Any(m => m.Id == user.Id))
        {
            return $"Пользователь {email} не состоит в проекте {task.Project.Id}. Сначала пригласите его в проект: /project_invite [{task.Project.Id}] [{email}] [роль?]";
        }

        var inviteId = await taskInviteStore.CreateAsync(taskId, user.IdTelegram.Value);
        var keyboard = new Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardMarkup
        {
            Keyboard =
            [
                [new Telegram.Bot.Types.ReplyMarkups.KeyboardButton($"/accept_task [{inviteId}]")],
                [new Telegram.Bot.Types.ReplyMarkups.KeyboardButton($"/decline_task [{inviteId}]")]
            ],
            ResizeKeyboard = true
        };
        await bot.SendMessage(user.IdTelegram.Value, $"Вам предлагается задача {taskId}.", replyMarkup: keyboard);
        return $"Приглашение на задачу отправлено пользователю {email}. ID: {inviteId}";
    }

    private static string[] Parse(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return Array.Empty<string>();
        var cleaned = Regex.Replace(input, @"^/task_assign\s*", "", RegexOptions.IgnoreCase).Trim();
        var matches = Regex.Matches(cleaned, @"\[(.*?)\]");
        return matches.Select(m => m.Groups[1].Value.Trim()).ToArray();
    }
}
