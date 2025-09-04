namespace Infrastructure.Configuration.Telegram;

/// <summary>
/// Реализация <see cref="ITelegramSettings"/> по умолчанию.
/// </summary>
public class TelegramSettings : ITelegramSettings
{
    /// <inheritdoc />
    public required string Token { get; set; }
}
