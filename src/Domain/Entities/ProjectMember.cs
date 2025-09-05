namespace Domain.Entities;

/// <summary>
/// Связующая сущность «проект — участник» с указанием роли.
/// </summary>
public class ProjectMember
{
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public ProjectRole Role { get; set; } = ProjectRole.Executor;
}

/// <summary>
/// Роль участника в рамках проекта.
/// </summary>
public enum ProjectRole
{
    Admin = 1,
    Manager = 2,
    Executor = 3
}

