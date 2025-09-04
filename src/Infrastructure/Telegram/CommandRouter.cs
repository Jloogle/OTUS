using Domain.Commands;
using Domain.Commands.Help;
using Domain.Commands.Profile;
using Domain.Commands.Project;
using Domain.Commands.Start;
using Domain.Commands.Task;
using Domain.Commands.Back;
using Domain.Constants;

namespace Infrastructure.Telegram;

/// <summary>
/// Маршрутизирует входящие команды к соответствующим обработчикам, найденным при старте.
/// </summary>
public class CommandRouter : ICommandRouting
{
    private readonly Dictionary<string, Func<ICommand, Task<string?>>> _routes = new();
    private const string CommandNotFoundError = "Команда не распознана";

    public CommandRouter(IEnumerable<object> handlers)
    {
        RegisterRoutes(handlers);
    }

    /// <summary>
    /// Регистрирует маршруты для всех известных пар команда/обработчик.
    /// </summary>
    private void RegisterRoutes(IEnumerable<object> handlers)
    {
        foreach (var handler in handlers)
        {
            RegisterRoute<StartCommand>(handler, BotCommands.Start);
            RegisterRoute<HelpCommand>(handler, BotCommands.Help);
            RegisterRoute<ProfileCommand>(handler, BotCommands.Profile);
            RegisterRoute<ProjectCommand>(handler, BotCommands.Project);
            RegisterRoute<AddProjectCommand>(handler, BotCommands.AddProject);
            RegisterRoute<DeleteProjectCommand>(handler, BotCommands.ProjectDelete);
            RegisterRoute<ListProjectCommand>(handler, BotCommands.ListMyProjects);
            RegisterRoute<InviteProjectMemberCommand>(handler, BotCommands.ProjectInvite);
            RegisterRoute<AcceptInviteCommand>(handler, BotCommands.AcceptInvite);
            RegisterRoute<DeclineInviteCommand>(handler, BotCommands.DeclineInvite);
            RegisterRoute<ChangeTaskCommand>(handler, BotCommands.ChangeTask);
            RegisterRoute<ListInvitesCommand>(handler, BotCommands.ListInvites);
            RegisterRoute<InviteHistoryCommand>(handler, BotCommands.InviteHistory);
            RegisterRoute<BackCommand>(handler, BotCommands.Back);
        }
    }

    /// <summary>
    /// Регистрирует маршрут для конкретного типа команды, если обработчик реализует интерфейс.
    /// </summary>
    private void RegisterRoute<TCommand>(object handler, string trigger) where TCommand : ICommand
    {
        if (handler is ICommandHandler<TCommand> specificHandler)
        {
            _routes.Add(trigger, async command => await specificHandler.Handle((TCommand)command));
        }
    }

    /// <summary>
    /// Находит обработчик команды и возвращает сообщение пользователю (если обработана).
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public async Task<string?> RouteAsync(ICommand command)
    {
        if (_routes.TryGetValue(command.Command, out var route))
        {
            return await route(command);
        }
        else
        {
            //TODO:Сделать обработку несуществующей команды
            Console.WriteLine(CommandNotFoundError);
        }

        return await Task.FromResult<string?>(null);
    }
}
