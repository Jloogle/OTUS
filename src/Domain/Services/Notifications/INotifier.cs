namespace Domain.Services.Notifications;

/// <summary>
/// Абстракция отправки текстовых уведомлений пользователю/в канал.
/// </summary>
public interface INotifier
{
    /// <summary>
    /// Отправить уведомление.
    /// </summary>
    Task NotifyAsync(long chatId, string message, CancellationToken ct = default);
}
