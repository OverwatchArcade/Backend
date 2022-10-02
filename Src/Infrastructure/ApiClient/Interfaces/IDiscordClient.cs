using OverwatchArcade.Persistence.Entities.Discord;

namespace OverwatchArcade.Persistence.ApiClient.Interfaces;

public interface IDiscordClient
{
    public Task<DiscordToken?> GetDiscordToken(string code, string redirectUri);
    public Task<DiscordLogin?> MakeDiscordOAuthCall(string token);
}