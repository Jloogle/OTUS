using Domain.Constants;

namespace Domain.Commands.Project;

/// <summary>
/// Команда для создания нового проекта.
/// </summary>
public class AddProjectCommand : ICommand
{
    public string Command => BotCommands.AddProject;
    public string? UserCommand { get; set; }
    public long? UserId { get; set; }
}