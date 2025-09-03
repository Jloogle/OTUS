using System.Text.Json;
using Domain.Services;
using Infrastructure.PostgreSQL;

namespace Infrastructure.Redis.Repository;

public class InviteStore(IAdapterMultiplexer adapterMultiplexer) : IInviteStore
{
    private readonly string CounterKey = "Invite:Counter";
    private static string InviteKey(int id) => $"Invite:{id}";
    private static string InviteeIndexKey(long tgId) => $"InviteIndex:Invitee:{tgId}";

    public async Task<int> CreateAsync(int projectId, long inviterTgId, long inviteeTgId, string? roleName)
    {
        var db = adapterMultiplexer.getMultiplexer().GetDatabase();
        var newId = (int)await db.StringIncrementAsync(CounterKey);
        var invite = new ProjectInvite(newId, projectId, inviterTgId, inviteeTgId, roleName, DateTime.UtcNow);
        await db.StringSetAsync(InviteKey(newId), JsonSerializer.Serialize(invite), TimeSpan.FromDays(7));
        await db.SetAddAsync(InviteeIndexKey(inviteeTgId), InviteKey(newId));
        return newId;
    }

    public async Task<ProjectInvite?> GetAsync(int inviteId)
    {
        var db = adapterMultiplexer.getMultiplexer().GetDatabase();
        var val = await db.StringGetAsync(InviteKey(inviteId));
        if (val.IsNullOrEmpty) return null;
        return JsonSerializer.Deserialize<ProjectInvite>(val!);
    }

    public async Task RemoveAsync(int inviteId)
    {
        var db = adapterMultiplexer.getMultiplexer().GetDatabase();
        var key = InviteKey(inviteId);
        var data = await db.StringGetAsync(key);
        if (!data.IsNullOrEmpty)
        {
            var inv = JsonSerializer.Deserialize<ProjectInvite>(data!);
            if (inv is not null)
            {
                await db.SetRemoveAsync(InviteeIndexKey(inv.InviteeTelegramId), key);
            }
        }
        await db.KeyDeleteAsync(key);
    }

    public async Task<IEnumerable<ProjectInvite>> ListActiveForInviteeAsync(long inviteeTelegramId)
    {
        var db = adapterMultiplexer.getMultiplexer().GetDatabase();
        var keys = await db.SetMembersAsync(InviteeIndexKey(inviteeTelegramId));
        var invites = new List<ProjectInvite>();
        foreach (var k in keys)
        {
            var val = await db.StringGetAsync(k.ToString());
            if (!val.IsNullOrEmpty)
            {
                var inv = JsonSerializer.Deserialize<ProjectInvite>(val!);
                if (inv is not null)
                    invites.Add(inv);
            }
            else
            {
                // Cleanup dangling index
                await db.SetRemoveAsync(InviteeIndexKey(inviteeTelegramId), k);
            }
        }
        return invites.OrderByDescending(i => i.CreatedAtUtc);
    }
}
