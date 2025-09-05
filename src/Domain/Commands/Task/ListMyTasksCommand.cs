using Domain.Constants;

namespace Domain.Commands.Task;

public class ListMyTasksCommand : ICommand
{
    public string Command => BotCommands.ListMyTasks;
    public long? UserId { get; set; }
}

