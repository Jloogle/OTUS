using System.Text.RegularExpressions;
using Domain.Commands;
using Domain.Commands.Task;
using Domain.Repositories;

namespace Application.CommandHandlers.ProjectTask;

/// <summary>
/// Обработчик установки/изменения дедлайна задачи.
/// </summary>
public class TaskSetDeadlineCommandHandler(ITaskRepository taskRepository) : ICommandHandler<TaskSetDeadlineCommand>
{
    public async Task<string?> Handle(TaskSetDeadlineCommand command)
    {
        var parts = Parse(command.UserCommand);
        if (parts.Length != 2) return "Установка дедлайна: /task_deadline [ID задачи] [ГГГГ-ММ-ДД( ЧЧ:ММ)?]";
        if (!int.TryParse(parts[0], out var taskId)) return "Некорректный ID задачи.";
        if (!DateTime.TryParse(parts[1], out var dl)) return "Некорректный формат даты.";

        var task = await taskRepository.GetByIdAsync(taskId);
        if (task == null) return $"Задача {taskId} не найдена.";
        task.Deadline = dl.Kind == DateTimeKind.Utc ? dl : dl.ToUniversalTime();
        await taskRepository.UpdateTaskAsync(task);
        return $"Дедлайн задачи {taskId} установлен на {dl:yyyy-MM-dd HH:mm}.";
    }

    private static string[] Parse(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return Array.Empty<string>();
        var cleaned = Regex.Replace(input, @"^/task_deadline\s*", "", RegexOptions.IgnoreCase).Trim();
        var matches = Regex.Matches(cleaned, @"\[(.*?)\]");
        return matches.Select(m => m.Groups[1].Value.Trim()).ToArray();
    }
}

