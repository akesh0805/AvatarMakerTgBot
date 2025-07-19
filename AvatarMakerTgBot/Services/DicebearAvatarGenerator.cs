using AvatarMakerTgBot.Abstractions;

namespace AvatarMakerTgBot.Services;

public class DicebearAvatarGenerator(HttpClient client, ILogger<DicebearAvatarGenerator> logger) : IAvatarGenerator
{
    public async ValueTask<byte[]> GenerateAvatarAsync(string seed, string style)
    {
        var url = $"https://api.dicebear.com/8.x/{style}/png?seed={Uri.EscapeDataString(seed)}";
        logger.LogInformation($"Fetching avatar: Style = {style}, Seed = {seed}");
        return await client.GetByteArrayAsync(url);
    }
}