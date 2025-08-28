using System.Collections.Concurrent;
using Domain.Commands;
using Domain.Commands.Help;
using Domain.Commands.Profile;
using Domain.Commands.Project;
using Domain.Commands.Start;
using Domain.Commands.Task;
using Domain.Constants;
using Infrastructure.Configuration.Telegram;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
namespace Infrastructure.Telegram;

public class TelegramBotHostedService(
    ITelegramSettings telegramSettings,
    ICommandRouting commandRouter)
    : BackgroundService
{
    private readonly ITelegramBotClient _botClient = new TelegramBotClient(telegramSettings.Token);
    private static readonly ConcurrentDictionary<int, ICommand> LastCommands = new();

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
    
    private static ICommand? ParseCommand(Update update)
    {
        if (update.Message?.Text is null) return null;
        
        var parts = update.Message.Text.Split(' ');
        var trigger = parts[0];
        
        if (!LastCommands.IsEmpty && trigger[0] != '/')
        {
             trigger = LastCommands.FirstOrDefault(x => x.Key == (int)update.Message.Chat.Id).Value.Command.ToString();
        }

        return trigger switch
        {
            BotCommands.Start => new StartCommand
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
            _ => null
        };
    }

    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Ошибка: {exception.Message}");
        return Task.CompletedTask;
    }
}