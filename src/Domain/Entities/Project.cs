namespace Domain.Entities;

/// <summary>
/// Проект: содержит участников и задачи, имеет общий дедлайн.
/// </summary>
public class Project
{
    /// <summary>Уникальный идентификатор проекта.</summary>
    public int Id { get; init; }
    /// <summary>Название проекта для идентификации.</summary>
    public string? Name { get; init; }
    /// <summary>Дата и время окончания проекта (дедлайн).</summary>
    public DateTime Deadline { get; init; }
    /// <summary>Список участников проекта.</summary>
    public List<User> Users { get; set; } = new List<User>();
    /// <summary>Список задач, принадлежащих проекту.</summary>
    public List<ProjTask> ProjTasks { get; set; } = new List<ProjTask>();
}
