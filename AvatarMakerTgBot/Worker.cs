using AvatarMakerTgBot.Abstractions;
using AvatarMakerTgBot.Models;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace AvatarMakerTgBot;

public class AvatarMakerTgBotWorker(ITelegramService telegramService, ILogger<AvatarMakerTgBotWorker> logger,IOptions<TelegramBotOptions> options) 
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _ = new TelegramBotClient(options.Value.Token);
        logger.LogInformation("🔁 AvatarMakerTgBot is starting...");
        await telegramService.StartAsync(stoppingToken);
    }
}
