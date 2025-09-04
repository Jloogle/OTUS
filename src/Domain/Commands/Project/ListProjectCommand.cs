using Domain.Constants;

namespace Domain.Commands.Project;

/// <summary>
/// Команда для просмотра списка проектов пользователя.
/// </summary>
public class ListProjectCommand : ICommand
{
    public string Command => BotCommands.ListMyProjects;

    public long? UserId { get; set; }
}