using System.Reflection;
using Application.CommandHandlers.Help;
using Application.CommandHandlers.Profile;
using Application.CommandHandlers.Project;
using Application.CommandHandlers.ProjectTask;
using Application.CommandHandlers.Start;
using Application.CommandHandlers.Back;
using Domain.Commands;
using Domain.Commands.Help;
using Domain.Commands.Profile;
using Domain.Commands.Project;
using Domain.Commands.Start;
using Domain.Commands.Task;
using Domain.Commands.Back;
using Infrastructure.Telegram;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions;

/// <summary>
/// Регистрирует обработчики команд уровня Application и маршрутизатор команд.
/// </summary>
public static class ServiceApplicationCollectionExtensions
{
    /// <summary>
    /// Добавляет сервисы приложения в контейнер DI.
    /// </summary>
    public static void AddApplicationServices(this IServiceCollection services)
    {
        
        services.AddTransient<ICommandHandler<StartCommand>, StartCommandHandler>();
        services.AddTransient<ICommandHandler<HelpCommand>, HelpCommandHandler>();
        services.AddTransient<ICommandHandler<ProfileCommand>, ProfileCommandHandler>();
        services.AddTransient<ICommandHandler<ProjectCommand>, ProjectCommandHandler>();
        services.AddTransient<ICommandHandler<AddProjectCommand>, AddProjectCommandHandler>();
        services.AddTransient<ICommandHandler<InviteProjectMemberCommand>, InviteProjectMemberCommandHandler>();
        services.AddTransient<ICommandHandler<DeleteProjectCommand>, DeleteProjectCommandHandler>();
        services.AddTransient<ICommandHandler<ListProjectCommand>, ListProjectCommandHandler>();
        services.AddTransient<ICommandHandler<ListInvitesCommand>, ListInvitesCommandHandler>();
        services.AddTransient<ICommandHandler<InviteHistoryCommand>, InviteHistoryCommandHandler>();
        services.AddTransient<ICommandHandler<ChangeTaskCommand>, ChangeTaskCommandHandler>();
        services.AddTransient<ICommandHandler<AcceptInviteCommand>, AcceptInviteCommandHandler>();
        services.AddTransient<ICommandHandler<DeclineInviteCommand>, DeclineInviteCommandHandler>();
        services.AddTransient<ICommandHandler<TaskCreateCommand>, TaskCreateCommandHandler>();
        services.AddTransient<ICommandHandler<BackCommand>, BackCommandHandler>();

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
