using Domain.Constants;

namespace Domain.Commands.Profile;

/// <summary>
/// Команда для вывода профиля пользователя.
/// </summary>
public class ProfileCommand : ICommand
{
    public string Command => BotCommands.Profile;

    public long? UserId { get; set; }
}