using Domain.Commands;
using Domain.Commands.Help;
using Domain.Commands.Profile;
using Domain.Commands.Project;
using Domain.Commands.Start;
using Domain.Constants;

namespace Infrastructure.Telegram;

public class CommandRouter : ICommandRouting
{
    private readonly Dictionary<string, Func<ICommand, Task<string?>>> _routes = new();
    private const string CommandNotFoundError = "Команда не распознана";

    public CommandRouter(IEnumerable<object> handlers)
    {
        RegisterRoutes(handlers);
    }
    
    private void RegisterRoutes(IEnumerable<object> handlers) 
    {
        foreach (var handler in handlers)
        {
            RegisterRoute<StartCommand>(handler, BotCommands.Start);
            RegisterRoute<HelpCommand>(handler, BotCommands.Help);
            RegisterRoute<ProfileCommand>(handler, BotCommands.Profile);
            RegisterRoute<ProjectCommand>(handler, BotCommands.Project);
        }
    }
    
    private void RegisterRoute<TCommand>(object handler, string trigger) where TCommand : ICommand
    {
        if (handler is ICommandHandler<TCommand> specificHandler)
        {
            _routes.Add(trigger, async command => await specificHandler.Handle((TCommand)command));
        }
    }

    /// <summary>
    /// Возвращает сообщение пользователю
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