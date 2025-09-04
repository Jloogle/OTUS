namespace Domain.Entities;

/// <summary>
/// Задача в рамках проекта с необязательным дедлайном и назначенными исполнителями.
/// </summary>
public class ProjTask
{
    /// <summary>Уникальный идентификатор задачи.</summary>
    public int Id { get; set; }
    /// <summary>Краткое название задачи.</summary>
    public string Name { get; set; }
    /// <summary>Подробное описание задачи и требований.</summary>
    public string Description { get; set; }
    /// <summary>Дата и время окончания задачи (опционально).</summary>
    public DateTime? Deadline { get; set; }

    // Связи с другими сущностями
    /// <summary>Проект, к которому принадлежит задача.</summary>
    public Project Project { get; set; } = null!;

    /// <summary>Пользователи, назначенные для выполнения задачи.</summary>
    public List<User> AssignedUsers { get; set; } = new List<User>();
}
