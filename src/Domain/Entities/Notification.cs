namespace Domain.Entities;

/// <summary>
/// Сущность уведомления, описывающая оповещения, связанные с задачами.
/// </summary>
public class Notification
{
    /// <summary>Уникальный идентификатор уведомления.</summary>
    public int Id { get; set; }
    /// <summary>Краткий заголовок уведомления.</summary>
    public string Name { get; set; }
    /// <summary>Дата и время создания уведомления.</summary>
    public DateTime Age { get; set; }
    /// <summary>Подробный текст уведомления.</summary>
    public string Description { get; set; }
    /// <summary>Список задач, связанных с уведомлением.</summary>
    public List<ProjTask> Tasks { get; set; } = new List<ProjTask>();
}
