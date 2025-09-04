using Domain.Constants;

namespace Domain.Commands.Help;

/// <summary>
/// Команда для вывода помощи по функциям бота.
/// </summary>
public class HelpCommand : ICommand
{
    public string Command => BotCommands.Help;

    public long? UserId { get; set; }
}