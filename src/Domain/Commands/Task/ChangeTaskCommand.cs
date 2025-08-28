using Domain.Constants;

namespace Domain.Commands.Task;

public class ChangeTaskCommand : ICommand
{
    public string Command => BotCommands.ChangeTask;
    public string? UserCommand { get; init; } 
    public long? UserId { get; set; } 
}