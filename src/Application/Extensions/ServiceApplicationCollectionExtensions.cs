
using System.Reflection;
using Domain.Commands;
using Infrastructure.Telegram;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions;

public static class ServiceApplicationCollectionExtensions
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var handlerType = typeof(ICommandHandler<>);

        var handlerTypes = assembly.GetTypes()
            .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerType))
            .ToList();

        foreach (var type in handlerTypes)
        {
            var interfaceType = type.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerType);
            services.AddTransient(interfaceType, type);
        }

        services.AddTransient<ICommandRouting, CommandRouter>(sp =>
        {
            var handlers = handlerTypes
                .SelectMany(t => t.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerType)
                    .Select(i => sp.GetService(i)))
                .Where(h => h != null);

            return new CommandRouter(handlers!);
        });
    }
}
