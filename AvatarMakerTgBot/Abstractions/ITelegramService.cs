namespace AvatarMakerTgBot.Abstractions;

public interface ITelegramService
{
    ValueTask StartAsync(CancellationToken cancellationToken);
}