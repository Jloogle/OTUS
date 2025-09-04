using Domain.Entities;
using Domain.Repositories;
using Domain.Services.Notifications;

namespace Infrastructure.Jobs;

/// <summary>
/// Фоновая задача: просматривает задачи и отправляет уведомления пользователям,
/// когда дедлайны приближаются согласно настроенным политикам.
/// </summary>
public class DeadlineNotifierJob(
    ITaskRepository taskRepository,
    IProjectRepository projectRepository,
    IDeadlineMessageFactory messageFactory,
    IEnumerable<IDeadlinePolicy> policies,
    INotifier notifier)
{
    /// <summary>
    /// Выполняет один проход проверки дедлайнов и отправку уведомлений.
    /// </summary>
    /// <param name="ct">Токен отмены.</param>
    public async Task Execute(CancellationToken ct = default)
    {
        var tasks = await taskRepository.GetAllTasksAsync();
        var now = DateTime.UtcNow;

        foreach (var task in tasks)
        {
            if (task.Deadline is null) continue;
            var deadlineUtc = task.Deadline.Value.Kind == DateTimeKind.Utc
                ? task.Deadline.Value
                : task.Deadline.Value.ToUniversalTime();

            foreach (var policy in policies)
            {
                if (!policy.ShouldNotify(now, deadlineUtc)) continue;

                var message = messageFactory.Create(task, policy);

                var recipients = new List<User>();
                if (task.AssignedUsers?.Count > 0)
                {
                    recipients.AddRange(task.AssignedUsers);
                }
                else if (task.Project is not null)
                {
                    var members = await projectRepository.GetProjectMembersAsync(task.Project.Id);
                    recipients.AddRange(members);
                }

                foreach (var user in recipients)
                {
                    if (user.IdTelegram is null) continue;
                    try
                    {
                        await notifier.NotifyAsync(user.IdTelegram.Value, message, ct);
                    }
                    catch
                    {
                        // Ignore individual failures, continue processing
                    }
                }
            }
        }
    }
}
