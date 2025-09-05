using Domain.Constants;

namespace Domain.Commands.Project;

/// <summary>
/// Команда для редактирования проекта (имя и дедлайн).
/// </summary>
public class EditProjectCommand : ICommand
{
    public string Command => BotCommands.ProjectEdit;
    public string? UserCommand { get; set; }
    public long? UserId { get; set; }
}

