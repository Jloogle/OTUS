namespace Domain.Entities;

/// <summary>
/// Проект: содержит участников и задачи, имеет общий дедлайн.
/// </summary>
public class Project
{
    /// <summary>Уникальный идентификатор проекта.</summary>
    public int Id { get; set; }
    /// <summary>Название проекта для идентификации.</summary>
    public string? Name { get; set; }
    /// <summary>Дата и время окончания проекта (дедлайн).</summary>
    public DateTime Deadline { get; set; }
    /// <summary>Список участников проекта (устаревшая связь многие-ко-многим, оставлена для обратной совместимости).</summary>
    public List<User> Users { get; set; } = new List<User>();
    /// <summary>Члены проекта с ролями.</summary>
    public List<ProjectMember> Members { get; set; } = new List<ProjectMember>();
    /// <summary>Список задач, принадлежащих проекту.</summary>
    public List<ProjTask> ProjTasks { get; set; } = new List<ProjTask>();
}
