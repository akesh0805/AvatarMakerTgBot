using AvatarMakerTgBot.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AvatarMakerTgBot.Services;

public class TelegramService(
    IAvatarGenerator avatarGenerator,
    ILogger<TelegramService> logger,
    Dictionary<string, string> supportedCommands,
    ITelegramBotClient botClient) : ITelegramService
{
    public async ValueTask StartAsync(CancellationToken cancellationToken)
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = []
        };

        botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            errorHandler: HandleErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cancellationToken
        );

        var me = await botClient.GetMe(cancellationToken);
        logger.LogInformation("Bot @{Username} is running...", me.Username);
    }

    private async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken token)
    {
        if (update.Message is not { Text: var messageText, Chat.Id: var chatId, From.Id: var userId }) return;

        logger.LogInformation("Request | UserId: {UserId} | Text: {Text}", userId, messageText);

        if (!string.IsNullOrEmpty(messageText) && messageText.StartsWith("/help", StringComparison.OrdinalIgnoreCase))
        {
            var helpText = "Avatar yaratish uchun quyidagi buyruqlardan foydalaning:\n" +
                        string.Join('\n', supportedCommands.Keys.Select(k => $"{k} <matn>")) +
                        "\n\nHar bir buyruqdan keyin matn yozing - bu avatar yaratilishiga asos bo'ladi.";

            await bot.SendMessage(
                chatId: chatId,
                text: helpText,
                cancellationToken: token);
            return;
        }

        if (supportedCommands.Keys.Any(cmd => messageText.StartsWith(cmd, StringComparison.OrdinalIgnoreCase)))
        {
            var parts = messageText.Split(' ', 2);
            var command = parts[0];
            var seed = parts.Length > 1 ? parts[1] : null;

            if (string.IsNullOrWhiteSpace(seed))
            {
                await bot.SendMessage(
                    chatId: chatId,
                    text: "Iltimos, buyruqdan keyin matn (seed) kiriting. Misol: /fun-emoji Ali",
                    cancellationToken: token);
                return;
            }

            var style = supportedCommands[command];
            try
            {
                var image = await avatarGenerator.GenerateAvatarAsync(seed, style);
                using var stream = new MemoryStream(image);

                await bot.SendPhoto(
                    chatId: chatId,
                    photo: new InputFileStream(stream, fileName: $"{style}.png"),
                    cancellationToken: token);

                logger.LogInformation("Sent avatar | UserId: {UserId} | Style: {Style} | Seed: {Seed}",
                    userId, style, seed);
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "Dicebear API error");
                await bot.SendMessage(
                    chatId: chatId,
                    text: "Avatar yaratishda xatolik yuz berdi. Keyinroq urinib ko'ring.",
                    cancellationToken: token);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "SendPhoto error");
                await bot.SendMessage(
                    chatId: chatId,
                    text: "Rasmni yuborishda xatolik yuz berdi.",
                    cancellationToken: token);
            }

            return;
        }
        await bot.SendMessage(
            chatId: chatId,
            text: "Iltimos, avatar olish uchun buyruqdan foydalaning. Misol: /bottts John",
            cancellationToken: token);
    }
    private Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken token)
    {
        logger.LogError(exception, "Telegram bot error");
        return Task.CompletedTask;
    }
}
