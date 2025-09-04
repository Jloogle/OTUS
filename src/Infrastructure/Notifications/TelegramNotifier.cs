using Domain.Services.Notifications;
using Telegram.Bot;

namespace Infrastructure.Notifications;

/// <summary>
/// Отправляет уведомления в чаты Telegram через <see cref="ITelegramBotClient"/>.
/// </summary>
public class TelegramNotifier(ITelegramBotClient bot) : INotifier
{
    /// <summary>
    /// Отправляет текстовое сообщение в чат Telegram.
    /// </summary>
    /// <param name="chatId">Идентификатор чата.</param>
    /// <param name="message">Текст сообщения.</param>
    /// <param name="ct">Токен отмены.</param>
    public async Task NotifyAsync(long chatId, string message, CancellationToken ct = default)
    {
        await bot.SendMessage(chatId, message, cancellationToken: ct);
    }
}
