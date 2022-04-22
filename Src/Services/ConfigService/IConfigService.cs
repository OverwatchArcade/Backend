using System.Collections.Generic;
using System.Threading.Tasks;
using OWArcadeBackend.Dtos.Contributor.Profile.About;
using OWArcadeBackend.Dtos.Contributor.Profile.Game.Overwatch.Portraits;
using OWArcadeBackend.Models;

namespace OWArcadeBackend.Services.ConfigService
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