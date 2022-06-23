using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using OverwatchArcade.API.Dtos;
using OverwatchArcade.Domain.Models;
using OverwatchArcade.Domain.Models.Constants;
using OverwatchArcade.Domain.Models.ContributorInformation.Game.Overwatch.Portraits;
using OverwatchArcade.Domain.Models.ContributorInformation.Personal;
using OverwatchArcade.Domain.Models.Overwatch;
using OverwatchArcade.Persistence;

namespace OverwatchArcade.API.Services.ConfigService
{
    public class ConfigService : IConfigService
    {
        private readonly ILogger<ConfigService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _memoryCache;
        private readonly IWebHostEnvironment _environment;

        public ConfigService(ILogger<ConfigService> logger, IUnitOfWork unitOfWork, IMemoryCache memoryCache, IWebHostEnvironment environment)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }
        
        public async Task<ServiceResponse<IEnumerable<Country>>> GetCountries()
        {
            var serviceResponse = new ServiceResponse<IEnumerable<Country>>();
            var config = await _unitOfWork.ConfigRepository.SingleOrDefaultASync(x => x.Key == ConfigKeys.COUNTRIES.ToString());
            
            if (config?.JsonValue == null)
            {
                serviceResponse.SetError(500, $"Config {ConfigKeys.COUNTRIES.ToString()} not found");
            }
            else
            {
                serviceResponse.Data = JsonConvert.DeserializeObject<IEnumerable<Country>>(config.JsonValue.ToString()) ?? throw new InvalidOperationException();
            }

            return serviceResponse;
        }

        public ServiceResponse<IEnumerable<ArcadeMode>> GetArcadeModes()
        {
            var arcadeModes = _unitOfWork.OverwatchRepository.GetArcadeModes(Game.OVERWATCH);
            return new ServiceResponse<IEnumerable<ArcadeMode>>
            {
                Data = arcadeModes
            };
        }

        public async Task<ServiceResponse<IEnumerable<HeroPortrait>>> GetOverwatchHeroes()
        {
            var serviceResponse = new ServiceResponse<IEnumerable<HeroPortrait>>();
            var config = await _unitOfWork.ConfigRepository.SingleOrDefaultASync(x => x.Key == ConfigKeys.OW_HEROES.ToString());
            
            if (config?.JsonValue == null)
            {
                serviceResponse.SetError(500, $"Config {ConfigKeys.OW_HEROES.ToString()} not found");
            }
            else
            {
                var heroes = JsonConvert.DeserializeObject<IEnumerable<HeroPortrait>>(config.JsonValue.ToString());
                serviceResponse.Data = heroes;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<IEnumerable<MapPortrait>>> GetOverwatchMaps()
        {
            var serviceResponse = new ServiceResponse<IEnumerable<MapPortrait>>();
            var config = await _unitOfWork.ConfigRepository.SingleOrDefaultASync(x => x.Key == ConfigKeys.OW_MAPS.ToString());
            
            if (config?.JsonValue == null)
            {
                serviceResponse.SetError(500, $"Config {ConfigKeys.OW_MAPS.ToString()} not found");
            }
            else
            {
                var maps = JsonConvert.DeserializeObject<IEnumerable<MapPortrait>>(config.JsonValue.ToString());
                serviceResponse.Data = maps;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<string?>> GetOverwatchEventWallpaper()
        {
            var serviceResponse = new ServiceResponse<string?>();

            try
            {
                var theme = await GetCurrentOverwatchEvent();
                var files = Directory.GetFiles(_environment.WebRootPath + ImageConstants.OwEventsFolder + theme.Data).Select(Path.GetFileName).ToList();
                var randomFile = files[new Random().Next(files.Count)];
                
                serviceResponse.Data = Environment.GetEnvironmentVariable("BACKEND_URL") + ImageConstants.OwEventsFolder + theme.Data + "/" + randomFile;
            }
            catch(Exception e)
            {
                _logger.LogCritical(e.Message);
                serviceResponse.SetError(500, "Couldn't get Overwatch event wallpaper URL");
            }

            return serviceResponse;
        }
        
        public async Task<ServiceResponse<string>> PostOverwatchEvent(string overwatchEvent)
        {
            var serviceResponse = new ServiceResponse<string>();
            var events = Directory
                .GetDirectories(_environment.WebRootPath + ImageConstants.OwEventsFolder)
                .Select(Path.GetFileName)
                .ToArray();

            if (!events.Contains(overwatchEvent))
            {
                serviceResponse.SetError(500, "Unknown event");
                return serviceResponse;
            }
            
            var config = await _unitOfWork.ConfigRepository.SingleOrDefaultASync(x => x.Key == ConfigKeys.OW_CURRENT_EVENT.ToString());
            
            if (config?.Value == null)
                serviceResponse.SetError(500, $"Config {ConfigKeys.OW_CURRENT_EVENT.ToString()} not found");

            config.Value = overwatchEvent;
            serviceResponse.Data = config.Value;
            await _unitOfWork.Save();

            _memoryCache.Set(CacheKeys.ConfigOverwatchEvent, serviceResponse);
            return serviceResponse;
        }

        public async Task<ServiceResponse<string?>> GetCurrentOverwatchEvent()
        {
            var serviceResponse = new ServiceResponse<string?>();
            var config = await _unitOfWork.ConfigRepository.SingleOrDefaultASync(x => x.Key == ConfigKeys.OW_CURRENT_EVENT.ToString());
            
            if (config?.Value == null)
                serviceResponse.SetError(500, $"Config {ConfigKeys.OW_CURRENT_EVENT.ToString()} not found");

            serviceResponse.Data = config.Value ?? "default";
            
            return serviceResponse;
        }

        public ServiceResponse<string?[]> GetOverwatchEvents()
        {
            var serviceResponse = new ServiceResponse<string?[]>
            {
                Data = Directory
                    .GetDirectories(_environment.WebRootPath + "/images/overwatch/events/")
                    .Select(Path.GetFileName)
                    .ToArray()
            };

            return serviceResponse;
        }
    }
}