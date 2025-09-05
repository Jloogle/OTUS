using Domain.Entities;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.PostgreSQL;

/// <summary>
/// Инициализирует базовые роли, если их нет: Admin, Manager, Executor.
/// </summary>
public class RoleSeederHostedService(IAdapterApplicationContext adapter) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var ctx = adapter.getContext();
        var names = new[] { "Admin", "Manager", "Executor" };
        foreach (var n in names)
        {
            if (!ctx.Roles.Any(r => r.Name == n))
            {
                ctx.Roles.Add(new Role { Name = n });
            }
        }
        await ctx.SaveChangesAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

