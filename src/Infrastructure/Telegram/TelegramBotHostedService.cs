using System.Collections.Concurrent;
using Domain.Commands;
using Domain.Commands.Help;
using Domain.Commands.Profile;
using Domain.Commands.Start;
using Domain.Constants;
using Infrastructure.Configuration.Telegram;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using TelegramTypes = Telegram.Bot.Types;

namespace Infrastructure.Telegram;

public class TelegramBotHostedService : BackgroundService
{
    private readonly ITelegramSettings _telegramSettings;
    private readonly ITelegramBotClient _botClient;
    private readonly ICommandRouting _commandRouter;
    private static readonly ConcurrentDictionary<int, ICommand> Lastcommands = new();
    
    public TelegramBotHostedService(
        ITelegramSettings telegramSettings,
        ICommandRouting commandRouter
    )
    {
        _telegramSettings = telegramSettings;
        _botClient = new TelegramBotClient(_telegramSettings.Token);
        _commandRouter = commandRouter;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        _botClient.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            new ReceiverOptions { AllowedUpdates = Array.Empty<UpdateType>() },
            cancellationToken: cancellationTokenSource.Token
        );
        await Task.CompletedTask;
    }
    
    private async Task HandleUpdateAsync(ITelegramBotClient botClient,TelegramTypes.Update update, CancellationToken cancellationToken)
    {
        if (update.Message is { Text: not null } message)
        {
            var command = ParseCommand(update) ?? Lastcommands.First(x=>x.Key == (int)update.Message.Chat.Id).Value;

            if (command != null)
            {
                Lastcommands.AddOrUpdate((int)update.Message.Chat.Id, command, (k, v) => command);
                var messageForUser = await _commandRouter.RouteAsync(command);
                if (messageForUser != null)
                {
                    await _botClient.SendMessage(
                        chatId: message.Chat.Id,
                        text: messageForUser,
                        cancellationToken: cancellationToken
                    );
                }
            }
        }
    }
    
    private static ICommand? ParseCommand(TelegramTypes.Update update)
    {
        if (update.Message?.Text is null) return null;
        
        var parts = update.Message.Text.Split(' ');
        var trigger = parts[0];

        return trigger switch
        {
            BotCommands.Start => new StartCommand
            {
                UserId = update.Message.From?.Id
            },
            BotCommands.Help => new HelpCommand
            {
                UserId = update.Message.From?.Id
            },
            BotCommands.Profile => new ProfileCommand
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