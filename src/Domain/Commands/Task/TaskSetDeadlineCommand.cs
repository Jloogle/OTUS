using Domain.Constants;

namespace Domain.Commands.Task;

/// <summary>
/// Команда установки/изменения дедлайна задачи.
/// </summary>
public class TaskSetDeadlineCommand : ICommand
{
    public string Command => BotCommands.TaskDeadline;
    public string? UserCommand { get; set; }
    public long? UserId { get; set; }
}

