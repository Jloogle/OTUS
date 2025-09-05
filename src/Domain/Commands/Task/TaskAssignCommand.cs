using Domain.Constants;

namespace Domain.Commands.Task;

public class TaskAssignCommand : ICommand
{
    public string Command => BotCommands.TaskAssign;
    public string? UserCommand { get; set; }
    public long? UserId { get; set; }
}

