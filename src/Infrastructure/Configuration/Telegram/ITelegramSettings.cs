namespace Infrastructure.Configuration.Telegram;

/// <summary>
/// Настройки бота Telegram, считываемые из конфигурации.
/// </summary>
public interface ITelegramSettings
{
    /// <summary>Токен доступа бота.</summary>
    string Token { get; set; }
}
