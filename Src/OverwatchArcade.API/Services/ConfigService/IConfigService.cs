using OverwatchArcade.API.Dtos;
using OverwatchArcade.Domain.Models.ContributorInformation.Game.Overwatch.Portraits;
using OverwatchArcade.Domain.Models.ContributorInformation.Personal;
using OverwatchArcade.Domain.Models.Overwatch;

namespace OverwatchArcade.API.Services.ConfigService
{
    public interface IConfigService
    {
        public ServiceResponse<IEnumerable<ArcadeMode>> GetArcadeModes();
        public Task<ServiceResponse<IEnumerable<Country>>> GetCountries();
        public Task<ServiceResponse<IEnumerable<HeroPortrait>>> GetOverwatchHeroes();
        public Task<ServiceResponse<IEnumerable<MapPortrait>>> GetOverwatchMaps();
        public Task<ServiceResponse<string?>> GetCurrentOverwatchEvent();
        public ServiceResponse<string?[]> GetOverwatchEvents();
        public Task<ServiceResponse<string?>> GetOverwatchEventWallpaper();
        public Task<ServiceResponse<string>> PostOverwatchEvent(string overwatchEvent);
    }
}