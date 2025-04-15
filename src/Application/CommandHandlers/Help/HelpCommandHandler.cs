using Domain.Commands;
using Domain.Commands.Help;
using Domain.Constants;


namespace Application.CommandHandlers.Help;

public class HelpCommandHandler : ICommandHandler<HelpCommand>
{
    public async Task<string> Handle(HelpCommand command)
    {
        
        return await Task.FromResult(BotCommands.PrintHelpMenu());
        
    }
}