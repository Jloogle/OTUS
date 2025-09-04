using Domain.Constants;

namespace Domain.Commands.Project;

/// <summary>
/// Команда для удаления проекта.
/// </summary>
public class DeleteProjectCommand : ICommand
{
    public string Command => BotCommands.ProjectDelete;

    public string? UserCommand { get; set; }
    public long? UserId { get; set; }
}