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
        if (parsedCommand.Length >= 2)
        {
            var action = parsedCommand[0].ToLower();
            
            if (action == "add" && parsedCommand.Length == 4)
            {
                var taskName = parsedCommand[1];
                var newDescription = parsedCommand[2];
                
                if (newDescription.Length>0)
                {
                    
                    await taskRepository.AddTaskAsync(new ProjTask() { Name = taskName, Description = newDescription }, Convert.ToInt32(parsedCommand[3]));
                    return $"Задача '{taskName}' успешно добавлена. ";
                }
                else
                {
                    return "Неверный формат даты дедлайна.";
                }
            }
            else if (action == "del" && parsedCommand.Length == 2)
            {
                if (int.TryParse(parsedCommand[1], out var taskId))
                {
                    await taskRepository.DeleteTaskAsync(taskId);
                    return $"Задача с ID {taskId} успешно удалена.";
                }
                else
                {
                    return "Неверный формат ID задачи.";
                }
            }
            else if (action == "assign" && parsedCommand.Length == 3)
            {
                if (int.TryParse(parsedCommand[1], out var taskId) && int.TryParse(parsedCommand[2], out var userId))
                {
                    await taskRepository.AssignTaskToUserAsync(taskId, userId);
                    return $"Пользователь с ID {userId} успешно назначен на задачу с ID {taskId}.";
                }
                else
                {
                    return "Неверный формат ID задачи или пользователя.";
                }
            }
            else if (parsedCommand.Length == 3 && int.TryParse(parsedCommand[0], out var updateTaskId))
            {
                var newTaskName = parsedCommand[1];
                var newDescription=parsedCommand[2];


                var projTask = await taskRepository.GetByIdAsync(updateTaskId);

                projTask.Name = newTaskName;
                projTask.Description = newDescription;
                    
                await taskRepository.UpdateTaskAsync(projTask);
                return $"Задача с ID {updateTaskId} успешно обновлена. Новое название: '{newTaskName}'.";

            }
        }
        
        return await Task.FromResult("Управление задачами:\n- Добавить: /change_task [add] [название задачи] [дедлайн]\n- Удалить: /change_task [del] [ID задачи]\n- Назначить: /change_task [assign] [ID задачи] [ID пользователя]\n- Изменить: /change_task [ID задачи] [новое название] [новый дедлайн]");
    }

    private string[] ParseChangeTaskCommand(string userCommand)
    {
        // Remove the "/add_project" part and trim any leading/trailing whitespace
        var cleanedCommand = Regex.Replace(userCommand, @"^/change_task\s*", "", RegexOptions.IgnoreCase).Trim();

        // Match content within square brackets
        var matches = Regex.Matches(cleanedCommand, @"\[(.*?)\]");

        // Extract matched content and return as array
        return matches.Select(m => m.Groups[1].Value.Trim()).ToArray();
    }
}