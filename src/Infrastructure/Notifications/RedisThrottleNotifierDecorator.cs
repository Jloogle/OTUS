using Domain.Services.Notifications;
using Infrastructure.PostgreSQL;
using StackExchange.Redis;

namespace Infrastructure.Notifications;

/// <summary>
/// Декоратор для <see cref="INotifier"/>, предотвращающий спам одинаковыми сообщениями одному и тому же
/// получателю в течение короткого периода времени (за счёт ключей с TTL в Redis).
/// </summary>
public class RedisThrottleNotifierDecorator(IAdapterMultiplexer multiplexer, INotifier inner) : INotifier
{
    private readonly IDatabase _db = multiplexer.getMultiplexer().GetDatabase();

    /// <summary>
    /// Отправляет сообщение, если идентичное не отправлялось недавно.
    /// Окно троттлинга — 15 минут на пару (чат, хэш сообщения).
    /// </summary>
    /// <param name="chatId">Идентификатор чата Telegram.</param>
    /// <param name="message">Текст сообщения.</param>
    /// <param name="ct">Токен отмены.</param>
    public async Task NotifyAsync(long chatId, string message, CancellationToken ct = default)
    {
        // Key based on recipient and message hash to avoid spamming
        var hash = message.GetHashCode().ToString("X");
        var key = $"Notify:Throttle:{chatId}:{hash}";
        // If key exists, skip; else set with TTL 15 minutes
        var existing = await _db.StringGetAsync(key);
        if (!existing.IsNullOrEmpty) return;
        await _db.StringSetAsync(key, "1", expiry: TimeSpan.FromMinutes(15));
        await inner.NotifyAsync(chatId, message, ct);
    }
}
