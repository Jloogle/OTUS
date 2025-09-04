namespace Domain.Services.Notifications;

/// <summary>
/// Стратегия, определяющая, нужно ли уведомлять по задаче с заданным дедлайном.
/// </summary>
public interface IDeadlinePolicy
{
    /// <summary>Человеко-читаемое имя политики.</summary>
    string Name { get; }
    /// <summary>Окно времени до дедлайна для срабатывания уведомления.</summary>
    TimeSpan Window { get; }
    /// <summary>
    /// Возвращает true, если уведомление должно быть отправлено в текущий момент для указанного дедлайна.
    /// </summary>
    bool ShouldNotify(DateTime utcNow, DateTime deadlineUtc);
}
