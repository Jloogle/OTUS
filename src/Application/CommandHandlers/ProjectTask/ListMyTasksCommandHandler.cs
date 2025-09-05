using Domain.Commands;
using Domain.Commands.Task;
using Domain.Repositories;

namespace Application.CommandHandlers.ProjectTask;

/// <summary>
/// Показывает задачи, назначенные текущему пользователю.
/// </summary>
public class ListMyTasksCommandHandler(ITaskRepository taskRepository) : ICommandHandler<ListMyTasksCommand>
{
    public async Task<string?> Handle(ListMyTasksCommand command)
    {
        var tasks = await taskRepository.GetTasksByUserIdAsync(command.UserId);
        if (tasks is null || !tasks.Any()) return "У вас нет назначенных задач.";
        var lines = tasks.Select(t => $"ID: {t.Id}, Название: {t.Name}, Описание: {t.Description}");
        return "Ваши задачи:\n" + string.Join("\n", lines);
    }
}

