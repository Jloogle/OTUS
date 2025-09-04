using Domain.Constants;

namespace Domain.Commands.Task;

/// <summary>
/// Команда для создания новой задачи в проекте.
/// </summary>
public class TaskCreateCommand : ICommand
{
    public string Command => BotCommands.TaskCreate;
    public string? UserCommand { get; set; }
    public long? UserId { get; set; }
}
