using Domain.Services.Notifications;

namespace Infrastructure.Notifications.Policies;

/// <summary>
/// Политика, срабатывающая для просроченных задач в пределах окна.
/// </summary>
public class OverdueDeadlinePolicy(TimeSpan window, string name) : IDeadlinePolicy
{
    /// <summary>Имя политики.</summary>
    public string Name { get; } = name;
    /// <summary>Окно времени после дедлайна, в течение которого уведомляем.</summary>
    public TimeSpan Window { get; } = window;

    /// <summary>
    /// Срабатывает, если дедлайн уже прошёл, но не позже, чем на Window.
    /// </summary>
    public bool ShouldNotify(DateTime utcNow, DateTime deadlineUtc)
    {
        var delta = utcNow - deadlineUtc; // положительное, если просрочено
        return delta > TimeSpan.Zero && delta <= Window;
    }
}

