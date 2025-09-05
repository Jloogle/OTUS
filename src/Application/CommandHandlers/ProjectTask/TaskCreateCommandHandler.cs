using System.Text.RegularExpressions;
using Domain.Commands;
using Domain.Commands.Task;
using Domain.Entities;
using Domain.Repositories;
using Domain.Services.Notifications;

namespace Application.CommandHandlers.ProjectTask;

/// <summary>
/// Создаёт новую задачу в проекте по команде бота.
/// </summary>
public class TaskCreateCommandHandler(ITaskRepository taskRepository, IProjectRepository projectRepository, INotifier notifier) : ICommandHandler<TaskCreateCommand>
{
    /// <summary>
    /// Разбирает параметры, создаёт задачу и возвращает подтверждение.
    /// </summary>
    public async Task<string?> Handle(TaskCreateCommand command)
    {
        var parts = Parse(command.UserCommand);
        if (parts.Length < 3) return Help();

        var name = parts[0];
        var desc = parts[1];
        if (!int.TryParse(parts[2], out var projectId)) return "Некорректный ID проекта.";

        var task = new ProjTask { Name = name, Description = desc };
        // Необязательный дедлайн
        if (parts.Length >= 4)
        {
            if (DateTime.TryParse(parts[3], out var dl))
            {
                task.Deadline = dl.Kind == DateTimeKind.Utc ? dl : dl.ToUniversalTime();
            }
            else
            {
                return "Некорректный формат дедлайна. Используйте ГГГГ-ММ-ДД или ГГГГ-ММ-ДД ЧЧ:ММ";
            }
        }
        var created = await taskRepository.AddTaskAsync(task, projectId);

        // Оповещаем участников проекта о новой задаче
        try
        {
            var members = await projectRepository.GetProjectMembersAsync(projectId);
            var text = $"Новая задача: '{created.Name}' в проекте {projectId}.";
            foreach (var u in members)
            {
                if (u.IdTelegram is { } chat)
                {
                    await notifier.NotifyAsync(chat, text);
                }
            }
        }
        catch { /* игнорируем сбои нотификаций */ }

        return $"Задача '{created.Name}' создана в проекте {projectId}, ID: {created.Id}.";
    }

    private static string Help() => "Создание задачи: /task_create [название] [описание] [ID проекта] [дедлайн?]";

    /// <summary>
    /// Извлекает параметры из ввода в квадратных скобках.
    /// </summary>
    private static string[] Parse(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return Array.Empty<string>();
        var cleaned = Regex.Replace(input, @"^/task_create\s*", "", RegexOptions.IgnoreCase).Trim();
        var matches = Regex.Matches(cleaned, @"\[(.*?)\]");
        return matches.Select(m => m.Groups[1].Value.Trim()).ToArray();
    }
}
