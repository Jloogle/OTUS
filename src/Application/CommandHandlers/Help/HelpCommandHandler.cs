using Domain.Commands;
using Domain.Commands.Help;

namespace Application.CommandHandlers.Help;

public class HelpCommandHandler : ICommandHandler<HelpCommand>
{
    public async Task<string> Handle(HelpCommand command)
    {
        return await Task.FromResult("Help");
    }
}