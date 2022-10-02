namespace OverwatchArcade.Persistence.Services.Interfaces;

public interface ILoginService
{
    public Task<string> Login(string discordAccessToken);
}