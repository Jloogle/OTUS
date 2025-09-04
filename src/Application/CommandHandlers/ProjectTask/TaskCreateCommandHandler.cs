using System.Text.RegularExpressions;
using Domain.Commands;
using Domain.Commands.Task;
using Domain.Entities;
using Domain.Repositories;

namespace Application.CommandHandlers.ProjectTask;

/// <summary>
/// Создаёт новую задачу в проекте по команде бота.
/// </summary>
public class TaskCreateCommandHandler(ITaskRepository taskRepository) : ICommandHandler<TaskCreateCommand>
{
    /// <summary>
    /// Разбирает параметры, создаёт задачу и возвращает подтверждение.
    /// </summary>
    public async Task<string?> Handle(TaskCreateCommand command)
    {
        var parts = Parse(command.UserCommand);
        if (parts.Length != 3) return Help();

        var name = parts[0];
        var desc = parts[1];
        if (!int.TryParse(parts[2], out var projectId)) return "Некорректный ID проекта.";

        var task = new ProjTask { Name = name, Description = desc };
        var created = await taskRepository.AddTaskAsync(task, projectId);
        return $"Задача '{created.Name}' создана в проекте {projectId}, ID: {created.Id}.";
    }

    private static string Help() => "Создание задачи: /task_create [название] [описание] [ID проекта]";

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
