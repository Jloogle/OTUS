using Infrastructure.Configuration.Telegram;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace Infrastructure.Extensions;

public static class ServiceInfrastructureCollectionExtensions
{
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
    }
}
