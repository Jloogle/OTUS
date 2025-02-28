namespace Infrastructure.Configuration.Telegram;

public class TelegramSettings : ITelegramSettings
{
    public required string Token { get; set; }
}