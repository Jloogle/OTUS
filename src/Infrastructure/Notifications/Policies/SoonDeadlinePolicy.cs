using Domain.Services.Notifications;

namespace Infrastructure.Notifications.Policies;

/// <summary>
/// Политика дедлайна, которая срабатывает для задач,
/// чей срок истекает в пределах заданного окна <see cref="Window"/> от текущего UTC времени.
/// </summary>
public class SoonDeadlinePolicy(TimeSpan window, string name) : IDeadlinePolicy
{
    /// <summary>
    /// Человеко-читаемое имя политики (например, "24 часа").
    /// </summary>
    public string Name { get; } = name;
    /// <summary>
    /// Временное окно до дедлайна, при котором следует уведомлять.
    /// </summary>
    public TimeSpan Window { get; } = window;

    /// <summary>
    /// Определяет, нужно ли отправить уведомление для указанного дедлайна.
    /// </summary>
    /// <param name="utcNow">Текущее время в UTC.</param>
    /// <param name="deadlineUtc">Дедлайн задачи в UTC.</param>
    /// <returns>
    /// True, если оставшееся время положительно и не больше <see cref="Window"/>.
    /// </returns>
    public bool ShouldNotify(DateTime utcNow, DateTime deadlineUtc)
    {
        var remaining = deadlineUtc - utcNow;
        return remaining > TimeSpan.Zero && remaining <= Window;
    }
}
