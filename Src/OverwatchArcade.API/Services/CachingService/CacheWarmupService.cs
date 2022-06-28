using Microsoft.Extensions.Caching.Memory;
using OverwatchArcade.API.Services.ConfigService;
using OverwatchArcade.API.Services.OverwatchService;
using OverwatchArcade.Domain.Models.Constants;

namespace OverwatchArcade.API.Services.CachingService
{
    public class CacheWarmupService : ICacheWarmupService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IConfigService _configService;
        private readonly ILogger<CacheWarmupService> _logger;
        private readonly IOverwatchService _overwatchService;

        public CacheWarmupService(IMemoryCache memoryCache, IConfigService configService, ILogger<CacheWarmupService> logger, IOverwatchService overwatchService)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _configService = configService ?? throw new ArgumentNullException(nameof(configService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _overwatchService = overwatchService ?? throw new ArgumentNullException(nameof(overwatchService));
        }

        /// <summary>
        /// Method used to cache service values when application starts
        /// Gets all values from <see cref="ConfigService"/> and <see cref="OverwatchService"/>
        /// Sets values in memory
        /// </summary>
        public async Task Run()
        {
            var endOfDayInUtc = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 23, 59, 59, 999);

            var countries = await _configService.GetCountries();

            var owHeroes = await _configService.GetOverwatchHeroes();
            var owMaps = await _configService.GetOverwatchMaps();
            var owEvent = await _configService.GetCurrentOverwatchEvent();
            var owEvents = _configService.GetOverwatchEvents();
            var owConfigArcadeModes = _configService.GetArcadeModes();

            var owDaily = _overwatchService.GetDaily();
            var owArcadeModes = _overwatchService.GetArcadeModes();
            var owLabels = _overwatchService.GetLabels();
            
            _memoryCache.Set(CacheKeys.Countries, countries);
            _memoryCache.Set(CacheKeys.ConfigOverwatchMaps, owMaps);
            _memoryCache.Set(CacheKeys.ConfigOverwatchEvent, owEvent);
            _memoryCache.Set(CacheKeys.ConfigOverwatchHeroes, owHeroes);
            _memoryCache.Set(CacheKeys.OverwatchLabels, owLabels);
            _memoryCache.Set(CacheKeys.ConfigOverwatchEvents, owEvents);
            _memoryCache.Set(CacheKeys.ConfigOverwatchArcadeModes, owConfigArcadeModes);
            _memoryCache.Set(CacheKeys.OverwatchArcadeModes, owArcadeModes);
            _memoryCache.Set(CacheKeys.OverwatchDaily, owDaily, endOfDayInUtc);
            
            _logger.LogInformation("Cache warmed!");
        }
    }
}