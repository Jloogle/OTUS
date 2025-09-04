using System.Collections.Concurrent;
using Domain.Commands;
using Domain.Commands.Help;
using Domain.Commands.Profile;
using Domain.Commands.Project;
using Domain.Commands.Start;
using Domain.Commands.Task;
using Domain.Commands.Back;
using Domain.Constants;
using Infrastructure.Configuration.Telegram;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
namespace Infrastructure.Telegram;

/// <summary>
/// Фоновый сервис, запускающий long polling Telegram-бота и маршрутизацию входящих команд.
/// </summary>
public class TelegramBotHostedService(
    ITelegramSettings telegramSettings,
    ICommandRouting commandRouter,
    ITelegramBotClient botClient)
    : BackgroundService
{
    private readonly ITelegramBotClient _botClient = botClient;
    private static readonly ConcurrentDictionary<int, ICommand> LastCommands = new();

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        _botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            errorHandler: HandleErrorAsync,
            receiverOptions: new ReceiverOptions { AllowedUpdates = Array.Empty<UpdateType>() },
            cancellationToken: cancellationTokenSource.Token
        );
        await Task.CompletedTask;
    }
    
    /// <summary>
    /// Обрабатывает входящие обновления Telegram и маршрутизирует команды.
    /// </summary>
    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Message is { Text: not null } message)
        {   
            var command = ParseCommand(update);

            if (command != null)
            {
                LastCommands.AddOrUpdate((int)update.Message.Chat.Id, command, (k, v) => command);
                var messageForUser = await commandRouter.RouteAsync(command);
                if (messageForUser != null)
                {
                    
                    var replyMarkup = RouterKeyboard(update.Message.Text);

                    await botClient.SendMessage(
                        chatId: message.Chat.Id,
                        text: messageForUser,
                        replyMarkup: replyMarkup,
                        cancellationToken: cancellationToken
                    );
                }
            }
        }
    }
    
    /// <summary>
    /// Формирует клавиатуру в зависимости от текущего контекста команды.
    /// </summary>
    private ReplyKeyboardMarkup RouterKeyboard(string? command)
    {
        ReplyKeyboardMarkup replyMarkup;
        if (command == BotCommands.Project)
        {
              replyMarkup = new ReplyKeyboardMarkup
               {
                   Keyboard =
                   [
                   [new KeyboardButton(BotCommands.AddProject), new KeyboardButton(BotCommands.ListMyProjects)],
                   [new KeyboardButton(BotCommands.ListMyTasks), new KeyboardButton(BotCommands.ChangeTask)],
                   [new KeyboardButton(BotCommands.ListInvites), new KeyboardButton(BotCommands.InviteHistory)],
                   [new KeyboardButton(BotCommands.Back), new KeyboardButton(BotCommands.ProjectDelete)]
                   ],
                   ResizeKeyboard = true
               };
        }
        else
        {
            replyMarkup = new ReplyKeyboardMarkup
            {
                Keyboard =
                [
                    [new KeyboardButton(BotCommands.Start), new KeyboardButton(BotCommands.Help)],
                    [new KeyboardButton(BotCommands.Profile), new KeyboardButton(BotCommands.Project)]
                ],
                ResizeKeyboard = true
            };
        }
        
        return replyMarkup;
    }
    
    /// <summary>
    /// Преобразует текст в экземпляр команды; поддерживает состояние,
    /// когда пользователь присылает текст без слэша — подставляется последняя команда.
    /// </summary>
    private static ICommand? ParseCommand(Update update)
    {
        if (update.Message?.Text is null) return null;
        
        var parts = update.Message.Text.Split(' ');
        var trigger = parts[0];
        
        if (!LastCommands.IsEmpty && trigger[0] != '/')
        {
             trigger = LastCommands.FirstOrDefault(x => x.Key == (int)update.Message.Chat.Id).Value.Command.ToString();
        }

        // alias mapping
        if (trigger == BotCommands.Register)
        {
            trigger = BotCommands.Start;
        }
        // Убрали алиас /project_create

        return trigger switch
        {
            BotCommands.Start => new StartCommand
            {
                UserId = update.Message.From?.Id,
                UserCommand = update.Message.Text
            },
            BotCommands.Register => new StartCommand
            {
                UserId = update.Message.From?.Id,
                UserCommand = update.Message.Text
            },
            BotCommands.Help => new HelpCommand
            {
                UserId = update.Message.From?.Id
            },
            BotCommands.Profile => new ProfileCommand
            {
                UserId = update.Message.From?.Id
            },
            BotCommands.Project => new ProjectCommand
            {
                UserId = update.Message.From?.Id
            },
            BotCommands.AddProject => new AddProjectCommand
            {
                UserId = update.Message.From?.Id,
                UserCommand = update.Message.Text
            },
            BotCommands.ProjectDelete => new DeleteProjectCommand
            {
                UserId = update.Message.From?.Id,
                UserCommand = update.Message.Text
            },
            BotCommands.ListMyProjects => new ListProjectCommand()
            {
                UserId = update.Message.From?.Id
            },
            BotCommands.ChangeTask => new ChangeTaskCommand()
            {
                UserId = update.Message.From?.Id,
                UserCommand = update.Message.Text
            },
            BotCommands.TaskCreate => new TaskCreateCommand
            {
                UserId = update.Message.From?.Id,
                UserCommand = update.Message.Text
            },
            BotCommands.ProjectInvite => new InviteProjectMemberCommand
            {
                UserId = update.Message.From?.Id,
                UserCommand = update.Message.Text
            },
            BotCommands.AcceptInvite => new AcceptInviteCommand
            {
                UserId = update.Message.From?.Id,
                UserCommand = update.Message.Text
            },
            BotCommands.DeclineInvite => new DeclineInviteCommand
            {
                UserId = update.Message.From?.Id,
                UserCommand = update.Message.Text
            },
            BotCommands.ListInvites => new ListInvitesCommand
            {
                UserId = update.Message.From?.Id,
                UserCommand = update.Message.Text
            },
            BotCommands.InviteHistory => new InviteHistoryCommand
            {
                UserId = update.Message.From?.Id,
                UserCommand = update.Message.Text
            },
            BotCommands.Back => new BackCommand
            {
                UserId = update.Message.From?.Id
            },
            _ => null
        };
    }

    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Ошибка: {exception.Message}");
        return Task.CompletedTask;
    }
}
