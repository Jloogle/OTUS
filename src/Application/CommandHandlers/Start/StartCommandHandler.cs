using Domain.Commands;
using Domain.Commands.Start;

namespace Application.CommandHandlers.Start;

public class StartCommandHandler : ICommandHandler<StartCommand>
{
    public async Task<string> Handle(StartCommand command)
    {
        return await Task.FromResult("Start");
    }
}