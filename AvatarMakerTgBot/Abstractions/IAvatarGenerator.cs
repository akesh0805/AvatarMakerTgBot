namespace AvatarMakerTgBot.Abstractions;

public interface IAvatarGenerator
{
    ValueTask<byte[]> GenerateAvatarAsync(string seed, string style);
}
