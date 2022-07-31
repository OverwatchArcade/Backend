using OverwatchArcade.API.Dtos.Discord;

namespace OverwatchArcade.API.Utility.Interfaces;

public interface IDiscordApiClient
{
    public Task<DiscordToken?> GetDiscordToken(string code, string redirectUri);
    public Task<DiscordLoginDto?> MakeDiscordOAuthCall(string token);
}