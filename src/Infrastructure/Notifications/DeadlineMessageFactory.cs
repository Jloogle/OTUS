using Domain.Entities;
using Domain.Services.Notifications;

namespace Infrastructure.Notifications;

/// <summary>
/// Формирует пользовательские текстовые сообщения о приближении дедлайнов задач
/// в соответствии с заданной политикой.
/// </summary>
public class DeadlineMessageFactory : IDeadlineMessageFactory
{
    /// <summary>
    /// Формирует человеко-читаемое уведомление по задаче и политике.
    /// </summary>
    /// <param name="task">Задача, о которой уведомляем.</param>
    /// <param name="policy">Политика, по которой сработало окно уведомления.</param>
    /// <returns>Локализованное сообщение, готовое к отправке пользователю.</returns>
    public string Create(ProjTask task, IDeadlinePolicy policy)
    {
        var now = DateTime.UtcNow;
        var deadline = task.Deadline!.Value.ToUniversalTime();
        var remaining = deadline - now;
        var abs = remaining.Duration();
        var remainingHuman = abs.TotalHours >= 1
            ? $"{Math.Floor(abs.TotalHours)} ч. {abs.Minutes} мин."
            : $"{abs.Minutes} мин.";

        var projectName = task.Project?.Name ?? "проект";
        if (remaining >= TimeSpan.Zero)
        {
            return $"Напоминание: задача '{task.Name}' в '{projectName}' истекает через {remainingHuman} (политика: {policy.Name}).";
        }
        else
        {
            return $"Внимание: задача '{task.Name}' в '{projectName}' просрочена на {remainingHuman} (политика: {policy.Name}).";
        }
    }
}
