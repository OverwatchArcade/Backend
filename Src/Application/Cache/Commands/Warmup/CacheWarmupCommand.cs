using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using OverwatchArcade.Application.Config.Queries.GetCountries;
using OverwatchArcade.Application.Config.Queries.GetCurrentEvent;
using OverwatchArcade.Application.Config.Queries.GetEvents;
using OverwatchArcade.Application.Config.Queries.GetHeroPortraits;
using OverwatchArcade.Application.Config.Queries.GetMapPortraits;
using OverwatchArcade.Application.Overwatch.ArcadeModes.Commands;
using OverwatchArcade.Application.Overwatch.Daily.Queries.GetDaily;
using OverwatchArcade.Application.Overwatch.Labels.Queries;
using OverwatchArcade.Domain.Enums;

namespace OverwatchArcade.Application.Cache.Commands.Warmup;

public record CacheWarmupCommand : IRequest;

public class CacheWarmupCommandHandler : IRequestHandler<CacheWarmupCommand>
{
    private readonly IMediator _mediator;
    private readonly IMemoryCache _memoryCache; 
    private readonly ILogger<CacheWarmupCommandHandler> _logger;

    public CacheWarmupCommandHandler(IMediator mediator, IMemoryCache memoryCache, ILogger<CacheWarmupCommandHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<Unit> Handle(CacheWarmupCommand request, CancellationToken cancellationToken)
    {
        var endOfDayInUtc = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 23, 59, 59, 999);

        var countries = await _mediator.Send(new GetCountriesQuery(), cancellationToken);
        
        var owHeroes = await _mediator.Send(new GetHeroPortraitsQuery(), cancellationToken);
        var owMaps = await _mediator.Send(new GetMapPortraitsQuery(), cancellationToken);
        var owEvent = await _mediator.Send(new GetCurrentEventQuery(), cancellationToken);
        var owEvents = await _mediator.Send(new GetEventsQuery(), cancellationToken);
        var owArcadeModes = await _mediator.Send(new GetArcadeModesQuery(), cancellationToken);

        var owDaily = await _mediator.Send(new GetDailyQuery(), cancellationToken);
        var owArcadeModeDtos = await _mediator.Send(new GetArcadeModesDtosQuery(), cancellationToken);
        var owLabels = await _mediator.Send(new GetLabelsQuery(), cancellationToken);
 
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
        
        return Unit.Value;
    }
}