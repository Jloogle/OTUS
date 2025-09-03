using Domain.Constants;

namespace Domain.Commands.Task;

public class TaskCreateCommand : ICommand
{
    public string Command => BotCommands.TaskCreate;
    public string? UserCommand { get; set; }
    public long? UserId { get; set; }
}

