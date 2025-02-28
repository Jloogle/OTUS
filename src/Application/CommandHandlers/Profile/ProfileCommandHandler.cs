using Domain.Commands;
using Domain.Commands.Profile;

namespace Application.CommandHandlers.Profile;

public class ProfileCommandHandler : ICommandHandler<ProfileCommand>
{
    public async Task<string> Handle(ProfileCommand command)
    {
        return await Task.FromResult("Profile");
    }
}