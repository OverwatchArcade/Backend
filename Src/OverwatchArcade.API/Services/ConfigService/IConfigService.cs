using OverwatchArcade.API.Dtos;
using OverwatchArcade.Domain.Models.ContributorProfile.Game.Overwatch.Portraits;
using OverwatchArcade.Domain.Models.ContributorProfile.Personal;

namespace OverwatchArcade.API.Services.ConfigService
{
    public interface IConfigService
    {
        public Task<ServiceResponse<IEnumerable<Country>>> GetCountries();
        public ServiceResponse<IEnumerable<ArcadeMode>> GetArcadeModes();
        public Task<ServiceResponse<IEnumerable<Hero>>> GetOverwatchHeroes();
        public Task<ServiceResponse<IEnumerable<Map>>> GetOverwatchMaps();
        public Task<ServiceResponse<string>> GetCurrentOverwatchEvent();
        public ServiceResponse<string[]> GetOverwatchEvents();
        public Task<ServiceResponse<string>> GetOverwatchEventWallpaper();
        public Task<ServiceResponse<string>> PostOverwatchEvent(string overwatchEvent);
    }
}