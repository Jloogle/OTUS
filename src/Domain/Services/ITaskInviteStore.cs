namespace Domain.Services;

/// <summary>
/// Легковесное хранилище приглашений на назначение задач.
/// </summary>
public record TaskInvite(
    int Id,
    int TaskId,
    long AssigneeTelegramId,
    DateTime CreatedAtUtc
);

public interface ITaskInviteStore
{
    Task<int> CreateAsync(int taskId, long assigneeTgId);
    Task<TaskInvite?> GetAsync(int inviteId);
    Task RemoveAsync(int inviteId);
}

