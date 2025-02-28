using System.Reflection;
using Application.CommandHandlers.Help;
using Application.CommandHandlers.Profile;
using Application.CommandHandlers.Start;
using Application.CommandRouting;
using Domain.Commands;
using Domain.Commands.Help;
using Domain.Commands.Profile;
using Domain.Commands.Start;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions;

public static class ServiceApplicationCollectionExtensions
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<ICommandHandler<StartCommand>, StartCommandHandler>();
        services.AddTransient<ICommandHandler<HelpCommand>, HelpCommandHandler>();
        services.AddTransient<ICommandHandler<ProfileCommand>, ProfileCommandHandler>();

        services.AddTransient<ICommandRouting, CommandRouter>(sp =>
        {
            var serviceProvider = sp;
            var handlerTypes = typeof(ICommandHandler<>); // Базовый интерфейс

            var allHandlers = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract) // Исключаем абстрактные классы и интерфейсы
                .SelectMany(t =>
                    t.GetInterfaces()
                        .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerTypes)
                        .Select(i => serviceProvider.GetServices(i))
                );

            return new CommandRouter(allHandlers.SelectMany(x => x)!);
        });

    }
}