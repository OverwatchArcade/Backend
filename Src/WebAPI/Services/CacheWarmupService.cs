using MediatR;
using Microsoft.Extensions.Caching.Memory;
using OverwatchArcade.Application.Common.Interfaces;
using OverwatchArcade.Application.Config.Queries.GetCountries;
using OverwatchArcade.Application.Config.Queries.GetCurrentEvent;
using OverwatchArcade.Application.Config.Queries.GetEvents;
using OverwatchArcade.Application.Config.Queries.GetHeroPortraits;
using OverwatchArcade.Application.Config.Queries.GetMapPortraits;
using OverwatchArcade.Application.Overwatch.ArcadeModes.Commands;
using OverwatchArcade.Application.Overwatch.Daily.Queries.GetDaily;
using OverwatchArcade.Application.Overwatch.Labels.Queries;
using OverwatchArcade.Domain.Enums;

namespace WebAPI.Services
{
    public class CacheWarmupService : ICacheWarmupService
    {
        private readonly IMediator _mediator;
        private readonly IMemoryCache _memoryCache; 
        private readonly ILogger<CacheWarmupService> _logger;

        public CacheWarmupService(IMediator mediator, IMemoryCache memoryCache, ILogger<CacheWarmupService> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Method used to cache service values in memory when application starts
        /// </summary>
        public async Task Run()
        {
            var endOfDayInUtc = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 23, 59, 59, 999);

            var countries = await _mediator.Send(new GetCountriesQuery());

            var owHeroes = await _mediator.Send(new GetHeroPortraitsQuery());
            var owMaps = await _mediator.Send(new GetMapPortraitsQuery());
            var owEvent = await _mediator.Send(new GetCurrentEventQuery());
            var owEvents = await _mediator.Send(new GetEventsQuery());
            var owArcadeModes = await _mediator.Send(new GetArcadeModesQuery());

            var owDaily = await _mediator.Send(new GetDailyQuery());
            var owArcadeModeDtos = await _mediator.Send(new GetArcadeModesDtosQuery());
            var owLabels = await _mediator.Send(new GetLabelsQuery());
            
            _memoryCache.Set(CacheKeys.Countries, countries);
            _memoryCache.Set(CacheKeys.ConfigOverwatchMaps, owMaps);
            _memoryCache.Set(CacheKeys.ConfigOverwatchEvent, owEvent);
            _memoryCache.Set(CacheKeys.ConfigOverwatchHeroes, owHeroes);
            _memoryCache.Set(CacheKeys.OverwatchLabels, owLabels);
            _memoryCache.Set(CacheKeys.ConfigOverwatchEvents, owEvents);
            _memoryCache.Set(CacheKeys.ConfigOverwatchArcadeModes, owArcadeModes);
            _memoryCache.Set(CacheKeys.OverwatchArcadeModesDtos, owArcadeModeDtos);
            _memoryCache.Set(CacheKeys.OverwatchDaily, owDaily, endOfDayInUtc);
            
            _logger.LogInformation("Cache warmed!");
        }
    }
}