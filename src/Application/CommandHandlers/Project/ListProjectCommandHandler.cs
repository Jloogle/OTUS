using System.Text;
using Domain.Commands;
using Domain.Commands.Project;
using Domain.Repositories;

public class ListProjectCommandHandler(IProjectRepository projectRepository) : ICommandHandler<ListProjectCommand>
{
    public async Task<string?> Handle(ListProjectCommand command)
    {
        var projects = await projectRepository.GetAllAsync();
        
        if (projects == null || !projects.Any())
            return "No projects found.";

        var result = new StringBuilder();
        result.AppendLine("ID | Deadline ");
        result.AppendLine(new string('-', 60));
        
        foreach (var project in projects)
        {
            result.AppendLine($"{project.Name} | {project.Deadline}");
        }
        
        return result.ToString();
    }
}