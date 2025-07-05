using System.Collections.Concurrent;
using Domain.Commands;
using Domain.Commands.Help;
using Domain.Commands.Profile;
using Domain.Commands.Project;
using Domain.Commands.Start;
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
                    var replyMarkup = new ReplyKeyboardMarkup
                    {
                        Keyboard = new []
                        {
                            new [] { new KeyboardButton(BotCommands.Start) },
                            new [] { new KeyboardButton(BotCommands.Help) },
                            new [] { new KeyboardButton(BotCommands.Profile) },
                            new [] { new KeyboardButton(BotCommands.Project) }
                        },
                        ResizeKeyboard = true
                    };

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
            _ => null
        };
    }

    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Ошибка: {exception.Message}");
        return Task.CompletedTask;
    }
}