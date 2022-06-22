using OverwatchArcade.API.Dtos;

namespace OverwatchArcade.API.Services.AuthService
{
    public interface IAuthService
    {
        public Task<ServiceResponse<string>> RegisterAndLogin(string discordBearerToken, string redirectUri);
    }
}