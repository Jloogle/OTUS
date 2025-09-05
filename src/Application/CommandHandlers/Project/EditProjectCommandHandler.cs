using System.Text.RegularExpressions;
using Domain.Commands;
using Domain.Commands.Project;
using Domain.Repositories;

namespace Application.CommandHandlers.Project;

/// <summary>
/// Обработчик редактирования проекта.
/// </summary>
public class EditProjectCommandHandler(IProjectRepository projectRepository) : ICommandHandler<EditProjectCommand>
{
    /// <summary>
    /// Ожидает формат: /project_edit [ID] [название] [ГГГГ-ММ-ДД]
    /// </summary>
    public async Task<string?> Handle(EditProjectCommand command)
    {
        var parts = Parse(command.UserCommand);
        if (parts.Length < 3) return "Редактирование проекта: /project_edit [ID] [название] [дедлайн]";

        if (!int.TryParse(parts[0], out var id)) return "Некорректный ID проекта.";
        var name = parts[1];
        if (!DateTime.TryParse(parts[2], out var deadline)) return "Некорректный формат даты (ожидается ГГГГ-ММ-ДД).";

        await projectRepository.UpdateProjectAsync(id, name, deadline);
        return $"Проект {id} обновлён: {name}, дедлайн {deadline:yyyy-MM-dd}.";
    }

    private static string[] Parse(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return Array.Empty<string>();
        var cleaned = Regex.Replace(input, @"^/project_edit\s*", "", RegexOptions.IgnoreCase).Trim();
        var matches = Regex.Matches(cleaned, @"\[(.*?)\]");
        return matches.Select(m => m.Groups[1].Value.Trim()).ToArray();
    }
}

