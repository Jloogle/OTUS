using Domain.Commands;
using Domain.Commands.Help;
using Domain.Constants;


namespace Application.CommandHandlers.Help;

/// <summary>
/// Показывает меню помощи со списком поддерживаемых команд.
/// </summary>
public class HelpCommandHandler : ICommandHandler<HelpCommand>
{
    /// <summary>
    /// Возвращает готовый текст помощи из констант.
    /// </summary>
    public async Task<string?> Handle(HelpCommand command)
    {
        
        return await Task.FromResult(BotCommands.PrintHelpMenu());
        
    }
}
