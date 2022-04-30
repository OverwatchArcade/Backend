using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OWArcadeBackend.Dtos.Contributor.Profile.Game.Overwatch.Portraits;
using OWArcadeBackend.Dtos.Contributor.Profile.Personal;
using OWArcadeBackend.Models;
using OWArcadeBackend.Models.Constants;
using OWArcadeBackend.Persistence;

namespace OWArcadeBackend.Services.ConfigService
{
    public class ConfigService : IConfigService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger _logger;
        private readonly IMemoryCache _memoryCache;

        public ConfigService(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment environment, ILogger<ConfigService> logger, IMemoryCache memoryCache)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
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
                serviceResponse.Data = JsonConvert.DeserializeObject<IEnumerable<Country>>(config.JsonValue.ToString());
            }

            return serviceResponse;
        }

        public ServiceResponse<IEnumerable<ArcadeMode>> GetArcadeModes()
        {
            var arcadeModes = _unitOfWork.OverwatchRepository.GetArcadeModes(Game.OVERWATCH);
            return new ServiceResponse<IEnumerable<ArcadeMode>>
            {
                Data = _mapper.Map<IEnumerable<ArcadeMode>>(arcadeModes)
            };
        }

        public async Task<ServiceResponse<IEnumerable<Hero>>> GetOverwatchHeroes()
        {
            var serviceResponse = new ServiceResponse<IEnumerable<Hero>>();
            var config = await _unitOfWork.ConfigRepository.SingleOrDefaultASync(x => x.Key == ConfigKeys.OW_HEROES.ToString());
            
            if (config?.JsonValue == null)
            {
                serviceResponse.SetError(500, $"Config {ConfigKeys.OW_HEROES.ToString()} not found");
            }
            else
            {
                var heroes = JsonConvert.DeserializeObject<IEnumerable<Hero>>(config.JsonValue.ToString());
                serviceResponse.Data = _mapper.Map<List<Hero>>(heroes);
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<IEnumerable<Map>>> GetOverwatchMaps()
        {
            var serviceResponse = new ServiceResponse<IEnumerable<Map>>();
            var config = await _unitOfWork.ConfigRepository.SingleOrDefaultASync(x => x.Key == ConfigKeys.OW_MAPS.ToString());
            
            if (config?.JsonValue == null)
            {
                serviceResponse.SetError(500, $"Config {ConfigKeys.OW_MAPS.ToString()} not found");
            }
            else
            {
                var maps = JsonConvert.DeserializeObject<IEnumerable<Map>>(config.JsonValue.ToString());
                serviceResponse.Data = _mapper.Map<List<Map>>(maps);
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<string>> GetOverwatchEventWallpaper()
        {
            var serviceResponse = new ServiceResponse<string>();

            try
            {
                var rand = new Random();
                var theme = await GetCurrentOverwatchEvent();
                var files = Directory.GetFiles(_environment.WebRootPath + "/images/overwatch/events/" + theme.Data + "/").Select(Path.GetFileName).ToList();
                var randomFile = files[rand.Next(files.Count)];
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
                .GetDirectories(_environment.WebRootPath + "/images/overwatch/events/")
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

        public async Task<ServiceResponse<string>> GetCurrentOverwatchEvent()
        {
            var serviceResponse = new ServiceResponse<string>();
            var config = await _unitOfWork.ConfigRepository.SingleOrDefaultASync(x => x.Key == ConfigKeys.OW_CURRENT_EVENT.ToString());
            
            if (config?.Value == null)
                serviceResponse.SetError(500, $"Config {ConfigKeys.OW_CURRENT_EVENT.ToString()} not found");

            serviceResponse.Data = config.Value;
            
            return serviceResponse;
        }

        public ServiceResponse<string[]> GetOverwatchEvents()
        {
            var serviceResponse = new ServiceResponse<string[]>
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