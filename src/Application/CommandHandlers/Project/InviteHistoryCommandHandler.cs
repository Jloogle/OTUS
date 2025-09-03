using Domain.Commands;
using Domain.Commands.Project;

namespace Application.CommandHandlers.Project;

public class InviteHistoryCommandHandler : ICommandHandler<InviteHistoryCommand>
{
    public async Task<string?> Handle(InviteHistoryCommand command)
    {
        // Placeholder: history persistence is not implemented yet
        return await Task.FromResult("История приглашений будет добавлена позже. Пока доступен список активных приглашений: /invites");
    }
}

