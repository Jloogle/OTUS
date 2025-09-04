using Domain.Commands;
using Domain.Commands.Project;

namespace Application.CommandHandlers.Project;

/// <summary>
/// Заглушка обработчика истории приглашений (будет реализовано при добавлении хранилища).
/// </summary>
public class InviteHistoryCommandHandler : ICommandHandler<InviteHistoryCommand>
{
    /// <summary>
    /// Возвращает дружелюбное сообщение-заглушку.
    /// </summary>
    public async Task<string?> Handle(InviteHistoryCommand command)
    {
        // Placeholder: history persistence is not implemented yet
        return await Task.FromResult("История приглашений будет добавлена позже. Пока доступен список активных приглашений: /invites");
    }
}
