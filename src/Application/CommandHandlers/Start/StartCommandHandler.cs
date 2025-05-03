using Domain.Commands;
using Domain.Commands.Start;
using Domain.Repositories;

namespace Application.CommandHandlers.Start;

public class StartCommandHandler(IUserRepository userRepository) : ICommandHandler<StartCommand>
{
    public async Task<string> Handle(StartCommand command)
    {
        return await Task.FromResult("Start");
    }
}