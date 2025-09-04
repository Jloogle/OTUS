using Domain.Constants;

namespace Domain.Commands.Project;

/// <summary>
/// Основная команда для работы с проектами.
/// </summary>
public class ProjectCommand : ICommand
{
    public string Command => BotCommands.Project;

    public long? UserId { get; set; }
}