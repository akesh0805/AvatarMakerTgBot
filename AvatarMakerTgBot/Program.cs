using AvatarMakerTgBot;
using AvatarMakerTgBot.Abstractions;
using AvatarMakerTgBot.Models;
using AvatarMakerTgBot.Services;
using Microsoft.Extensions.Options;
using Telegram.Bot;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<TelegramBotOptions>(
    builder.Configuration.GetSection("TelegramBot"));

builder.Services.Configure<SupportedCommandsOptions>(
    builder.Configuration.GetSection("SupportedCommands"));

builder.Services.AddHttpClient();

builder.Services.AddSingleton<ITelegramBotClient>(sp =>
{
    var config = sp.GetRequiredService<IOptions<TelegramBotOptions>>().Value;
    return new TelegramBotClient(config.Token);
});

builder.Services.AddSingleton<IAvatarGenerator, DicebearAvatarGenerator>();
builder.Services.AddSingleton<ITelegramService, TelegramService>();

builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IOptions<SupportedCommandsOptions>>().Value.Commands);

builder.Services.AddHostedService<AvatarMakerTgBotWorker>();

var host = builder.Build();
host.Run();
