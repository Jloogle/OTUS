
using System.Text.RegularExpressions;
using Domain.Commands;
using Domain.Commands.Task;
using Domain.Entities;
using Domain.Repositories;

namespace Application.CommandHandlers.ProjectTask;

public class ChangeTaskCommandHandler(ITaskRepository taskRepository) : ICommandHandler<ChangeTaskCommand>
{
    public async Task<string?> Handle(ChangeTaskCommand command)
    {
        var parsedCommand = ParseChangeTaskCommand(command.UserCommand);
        if (parsedCommand.Length == 0)
        {
            return GetHelpMessage();
        }

        var action = parsedCommand[0].ToLower();
        
        return action switch
        {
            "add" when parsedCommand.Length == 4 => await HandleAddTask(parsedCommand),
            "del" when parsedCommand.Length == 2 => await HandleDeleteTask(parsedCommand),
            "assign" when parsedCommand.Length == 3 => await HandleAssignTask(parsedCommand),
            "list" when parsedCommand.Length == 1 => await HandleListTasks(),
            "my" when parsedCommand.Length == 1 => await HandleMyTasks(command.UserId),
            _ when parsedCommand.Length == 3 && int.TryParse(parsedCommand[0], out _) => await HandleUpdateTask(parsedCommand),
            _ => GetHelpMessage()
        };
    }

    private async Task<string> HandleAddTask(string[] parsedCommand)
    {
        var taskName = parsedCommand[1];
        var description = parsedCommand[2];
        
        if (string.IsNullOrWhiteSpace(description))
        {
            return "Описание задачи не может быть пустым.";
        }

        if (!int.TryParse(parsedCommand[3], out var projectId))
        {
            return "Неверный формат ID проекта.";
        }

        var task = new ProjTask { Name = taskName, Description = description };
        await taskRepository.AddTaskAsync(task, projectId);
        return $"Задача '{taskName}' успешно добавлена.";
    }

    private async Task<string> HandleDeleteTask(string[] parsedCommand)
    {
        if (!int.TryParse(parsedCommand[1], out var taskId))
        {
            return "Неверный формат ID задачи.";
        }

        await taskRepository.DeleteTaskAsync(taskId);
        return $"Задача с ID {taskId} успешно удалена.";
    }

    private async Task<string> HandleAssignTask(string[] parsedCommand)
    {
        if (!int.TryParse(parsedCommand[1], out var taskId) || !int.TryParse(parsedCommand[2], out var userId))
        {
            return "Неверный формат ID задачи или пользователя.";
        }

        await taskRepository.AssignTaskToUserAsync(taskId, userId);
        return $"Пользователь с ID {userId} успешно назначен на задачу с ID {taskId}.";
    }

    private async Task<string> HandleListTasks()
    {
        var allTasks = await taskRepository.GetAllTasksAsync();
        if (!allTasks.Any())
        {
            return "Задачи не найдены.";
        }

        var taskList = string.Join("\n", allTasks.Select(t => 
            $"ID: {t.Id}, Название: {t.Name}, Описание: {t.Description}, Назначен: {(t.AssignedUsers.FirstOrDefault()?.ToString() ?? "Не назначен")}"));
        return $"Все задачи:\n{taskList}";
    }

    private async Task<string> HandleMyTasks(long? commandUserId)
    {


        var userTasks = await taskRepository.GetTasksByUserIdAsync(commandUserId);
        if (!userTasks.Any())
        {
            return "У вас нет назначенных задач.";
        }

        var taskList = string.Join("\n", userTasks.Select(t => 
            $"ID: {t.Id}, Название: {t.Name}, Описание: {t.Description}"));
        return $"Ваши задачи:\n{taskList}";
    }

    private async Task<string> HandleUpdateTask(string[] parsedCommand)
    {
        if (!int.TryParse(parsedCommand[0], out var taskId))
        {
            return "Неверный формат ID задачи.";
        }

        var newTaskName = parsedCommand[1];
        var newDescription = parsedCommand[2];

        var projTask = await taskRepository.GetByIdAsync(taskId);
        if (projTask == null)
        {
            return $"Задача с ID {taskId} не найдена.";
        }

        projTask.Name = newTaskName;
        projTask.Description = newDescription;
        
        await taskRepository.UpdateTaskAsync(projTask);
        return $"Задача с ID {taskId} успешно обновлена. Новое название: '{newTaskName}'.";
    }

    private static string GetHelpMessage()
    {
        return "Управление задачами:\n" +
               "- Добавить: /change_task [add] [название задачи] [описание] [ID проекта]\n" +
               "- Удалить: /change_task [del] [ID задачи]\n" +
               "- Назначить: /change_task [assign] [ID задачи] [ID пользователя]\n" +
               "- Изменить: /change_task [ID задачи] [новое название] [новое описание]\n" +
               "- Все задачи: /change_task [list]\n" +
               "- Мои задачи: /change_task [my]";
    }

    private string[] ParseChangeTaskCommand(string userCommand)
    {
        var cleanedCommand = Regex.Replace(userCommand, @"^/change_task\s*", "", RegexOptions.IgnoreCase).Trim();
        var matches = Regex.Matches(cleanedCommand, @"\[(.*?)\]");
        return matches.Select(m => m.Groups[1].Value.Trim()).ToArray();
    }
}
