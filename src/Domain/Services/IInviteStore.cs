namespace Domain.Services;

public record ProjectInvite(
    int Id,
    int ProjectId,
    long InviterTelegramId,
    long InviteeTelegramId,
    string? RoleName,
    DateTime CreatedAtUtc
);

public interface IInviteStore
{
    Task<int> CreateAsync(int projectId, long inviterTgId, long inviteeTgId, string? roleName);
    Task<ProjectInvite?> GetAsync(int inviteId);
    Task RemoveAsync(int inviteId);
    Task<IEnumerable<ProjectInvite>> ListActiveForInviteeAsync(long inviteeTelegramId);
}
