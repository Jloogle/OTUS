using Domain.Commands;
using Domain.Commands.Project;
using Domain.Repositories;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Application.CommandHandlers.Project;

public class AddProjectCommandHandler(IProjectRepository projectRepository) : ICommandHandler<AddProjectCommand>
{
    public async Task<string?> Handle(AddProjectCommand command)
    {
        var parsedCommand = ParseAddProjectCommand(command.UserCommand);
        if (parsedCommand.Length == 2)
        {
            var projectName = parsedCommand[0];
            var deadline = ConvertToDateTime(parsedCommand[1]);

            if (deadline.HasValue)
            {
                var id = await projectRepository.AddProjectAsync(projectName, deadline.Value);
                return $"Проект '{projectName}' успешно добавлен с дедлайном {deadline.Value.ToShortDateString()}. ID: {id}.";
            }
            else
            {
                return "Неверный формат даты дедлайна.";
            }
        }
        
        return await Task.FromResult("Создание проекта - /add_project [название задачи] [дедлайн].");
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

        // Match content within square brackets
        var matches = Regex.Matches(cleanedCommand, @"\[(.*?)\]");

        // Extract matched content and return as array
        return matches.Select(m => m.Groups[1].Value.Trim()).ToArray();
    }
}