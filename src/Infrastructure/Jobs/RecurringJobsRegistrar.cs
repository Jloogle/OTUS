using Hangfire;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Jobs;

/// <summary>
/// Регистрирует периодические фоновые задания (Hangfire) при запуске приложения.
/// </summary>
public class RecurringJobsRegistrar(IRecurringJobManager manager) : IHostedService
{
    /// <summary>
    /// Планирует периодические задания.
    /// </summary>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        manager.AddOrUpdate<DeadlineNotifierJob>(
            recurringJobId: "deadline-notifier",
            methodCall: job => job.Execute(CancellationToken.None),
            cronExpression: Cron.Minutely);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Действий при остановке не требуется.
    /// </summary>
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
