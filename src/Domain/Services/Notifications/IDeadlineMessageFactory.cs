using Domain.Entities;

namespace Domain.Services.Notifications;

/// <summary>
/// Фабрика сообщений, формирующая тексты уведомлений о дедлайнах.
/// </summary>
public interface IDeadlineMessageFactory
{
    /// <summary>
    /// Создаёт сообщение, описывающее оставшееся время по задаче согласно политике.
    /// </summary>
    string Create(ProjTask task, IDeadlinePolicy policy);
}
