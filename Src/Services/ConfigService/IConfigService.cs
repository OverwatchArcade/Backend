using System.Collections.Generic;
using System.Threading.Tasks;
using OWArcadeBackend.Models;

namespace OWArcadeBackend.Services.ConfigService
{
    public interface IConfigService
    {
        public Task<ServiceResponse<IEnumerable<ConfigCountries>>> GetCountries();
        public Task<ServiceResponse<IEnumerable<ConfigOverwatchHero>>> GetOverwatchHeroes();
        public Task<ServiceResponse<IEnumerable<ConfigOverwatchMap>>> GetOverwatchMaps();
        public Task<ServiceResponse<string>> GetCurrentOverwatchEvent();
        public ServiceResponse<string[]> GetOverwatchEvents();
        public Task<ServiceResponse<string>> GetOverwatchEventWallpaper();
        public Task<ServiceResponse<string>> PostOverwatchEvent(string overwatchEvent);
    }
}