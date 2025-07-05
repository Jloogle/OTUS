using Domain.Constants;

namespace Domain.Commands.Start;

public class StartCommand : ICommand
{
    public string Command
    {
        get => BotCommands.Start;
        set => throw new NotImplementedException();
    }

    public long? UserId { get; set; } 
    public string? UserCommand { get; set; } 
}