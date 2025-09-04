using Domain.Commands;
using Domain.Commands.Project;
using Domain.Repositories;

namespace Application.CommandHandlers.Project;

/// <summary>
/// Удаляет проект по его идентификатору.
/// </summary>
public class DeleteProjectCommandHandler : ICommandHandler<DeleteProjectCommand>
{
    private readonly IProjectRepository _projectRepository;

    public DeleteProjectCommandHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    /// <summary>
    /// Разбирает ID проекта и удаляет его через репозиторий.
    /// </summary>
    public async Task<string?> Handle(DeleteProjectCommand command)
    {
        var projectIdMatch = System.Text.RegularExpressions.Regex.Match(command.UserCommand, @"\[(\d+)\]");
        
        if (!projectIdMatch.Success || !int.TryParse(projectIdMatch.Groups[1].Value, out int projectId))
        {
            return "Некорректный формат команды. Используйте: /project_delete [ID]";
        }

        if (projectId <= 0)
        {
            return "Некорректный ID проекта.";
        }

        

        await _projectRepository.RemoveProjectAsync(projectId);
        
        return $"Проект (ID: {projectId}) успешно удален.";
    }
}
