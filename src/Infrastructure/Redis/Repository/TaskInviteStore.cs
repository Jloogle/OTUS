using System.Text.Json;
using Domain.Services;
using Infrastructure.PostgreSQL;

namespace Infrastructure.Redis.Repository;

/// <summary>
/// Хранилище инвайтов на назначение задач (Redis).
/// </summary>
public class TaskInviteStore(IAdapterMultiplexer adapterMultiplexer) : ITaskInviteStore
{
    private const string CounterKey = "TaskInvite:Counter";
    private static string InviteKey(int id) => $"TaskInvite:{id}";

    public async Task<int> CreateAsync(int taskId, long assigneeTgId)
    {
        var db = adapterMultiplexer.getMultiplexer().GetDatabase();
        var newId = (int)await db.StringIncrementAsync(CounterKey);
        var invite = new TaskInvite(newId, taskId, assigneeTgId, DateTime.UtcNow);
        await db.StringSetAsync(InviteKey(newId), JsonSerializer.Serialize(invite), TimeSpan.FromDays(3));
        return newId;
    }

    public async Task<TaskInvite?> GetAsync(int inviteId)
    {
        var db = adapterMultiplexer.getMultiplexer().GetDatabase();
        var val = await db.StringGetAsync(InviteKey(inviteId));
        if (val.IsNullOrEmpty) return null;
        return JsonSerializer.Deserialize<TaskInvite>(val!);
    }

    public async Task RemoveAsync(int inviteId)
    {
        var db = adapterMultiplexer.getMultiplexer().GetDatabase();
        await db.KeyDeleteAsync(InviteKey(inviteId));
    }
}

