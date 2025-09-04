namespace Domain.Services;

/// <summary>
/// Легковесное хранилище приглашений в проекты (например, Redis).
/// </summary>
public record ProjectInvite(
    int Id,
    int ProjectId,
    long InviterTelegramId,
    long InviteeTelegramId,
    string? RoleName,
    DateTime CreatedAtUtc
);

/// <summary>
/// Операции по созданию, чтению и удалению активных приглашений.
/// </summary>
public interface IInviteStore
{
    /// <summary>Создать новое приглашение и вернуть его идентификатор.</summary>
    Task<int> CreateAsync(int projectId, long inviterTgId, long inviteeTgId, string? roleName);
    /// <summary>Получить приглашение по идентификатору.</summary>
    Task<ProjectInvite?> GetAsync(int inviteId);
    /// <summary>Удалить приглашение по идентификатору.</summary>
    Task RemoveAsync(int inviteId);
    /// <summary>Список активных приглашений для указанного приглашённого.</summary>
    Task<IEnumerable<ProjectInvite>> ListActiveForInviteeAsync(long inviteeTelegramId);
}
