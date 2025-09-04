using Infrastructure.Configuration.Telegram;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Hangfire;
using Hangfire.PostgreSql;
using Telegram.Bot;
using Domain.Services.Notifications;
using Infrastructure.Notifications;
using Infrastructure.Notifications.Policies;
using Infrastructure.Jobs;
using Infrastructure.PostgreSQL;

namespace Infrastructure.Extensions;

/// <summary>
/// Регистрирует инфраструктуру: Telegram, Hangfire, уведомления и фоновые задания.
/// </summary>
public static class ServiceInfrastructureCollectionExtensions
{
    /// <summary>
    /// Добавляет инфраструктурные сервисы в DI, используя переданную конфигурацию.
    /// </summary>
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .Configure<TelegramSettings>(options => configuration.GetSection(nameof(TelegramSettings)).Bind(options));

        services
            .AddSingleton<ITelegramSettings>(sp => sp.GetRequiredService<IOptions<TelegramSettings>>().Value);

        services.AddSingleton<ITelegramBotClient>(sp =>
        {
            var settings = sp.GetRequiredService<ITelegramSettings>();
            return new TelegramBotClient(settings.Token);
        });

        // Hangfire configuration
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddHangfire(cfg =>
        {
            cfg.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
               .UseSimpleAssemblyNameTypeSerializer()
               .UseRecommendedSerializerSettings();
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                cfg.UsePostgreSqlStorage(connectionString);
            }
        });
        services.AddHangfireServer();

        // Notifications: Strategy + Factory + Decorator
        services.AddSingleton<IDeadlineMessageFactory, DeadlineMessageFactory>();
        services.AddSingleton<IDeadlinePolicy>(sp => new SoonDeadlinePolicy(TimeSpan.FromDays(1), "24 часа"));
        services.AddSingleton<IDeadlinePolicy>(sp => new SoonDeadlinePolicy(TimeSpan.FromHours(1), "1 час"));
        services.AddSingleton<IDeadlinePolicy>(sp => new SoonDeadlinePolicy(TimeSpan.FromMinutes(10), "10 минут"));
        services.AddSingleton<TelegramNotifier>();
        services.AddSingleton<INotifier>(sp =>
            new RedisThrottleNotifierDecorator(
                sp.GetRequiredService<IAdapterMultiplexer>(),
                sp.GetRequiredService<TelegramNotifier>()));

        // Register jobs
        services.AddTransient<DeadlineNotifierJob>();
        // Register recurring jobs registrar
        services.AddHostedService<RecurringJobsRegistrar>();
    }
}
