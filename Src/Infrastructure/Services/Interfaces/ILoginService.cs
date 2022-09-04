using OverwatchArcade.Persistence.Entities;

namespace OverwatchArcade.Persistence.Services.Interfaces;

public interface ILoginService
{
    public Task<ServiceResponse<string>> Login(string discordBearerToken, string redirectUri);
}