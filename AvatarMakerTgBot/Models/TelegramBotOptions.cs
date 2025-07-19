// Application/Options/TelegramBotOptions.cs
namespace AvatarMakerTgBot.Models;

public class TelegramBotOptions
{
    public string Token { get; set; } = string.Empty;
}

public class SupportedCommandsOptions
{
    public Dictionary<string, string> Commands { get; set; } = new();
}

