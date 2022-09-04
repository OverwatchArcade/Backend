using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OverwatchArcade.Application.Common.Exceptions;
using OverwatchArcade.Application.Common.Interfaces;
using OverwatchArcade.Domain.Constants;
using OverwatchArcade.Domain.Entities.ContributorInformation.Game.Overwatch.Portraits;
using OverwatchArcade.Domain.Enums;

namespace OverwatchArcade.Application.Config.Queries.GetMapPortraits;

public record GetMapPortraitsQuery : IRequest<IEnumerable<MapPortrait>>;

public class GetMapPortraitsQueryHandler : IRequestHandler<GetMapPortraitsQuery, IEnumerable<MapPortrait>>
{
    private readonly IApplicationDbContext _context;

    public GetMapPortraitsQueryHandler(IApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<MapPortrait>> Handle(GetMapPortraitsQuery request, CancellationToken cancellationToken)
    {
        var mapPortraits = await _context.Config
            .FirstOrDefaultAsync(c => c.Key.Equals(ConfigKeys.OwMaps.ToString()), cancellationToken);
        if (mapPortraits?.JsonValue == null)
        {
            throw new ConfigNotFoundException($"Config {ConfigKeys.OwMaps.ToString()} not found");
        }
        
        return JsonConvert.DeserializeObject<IEnumerable<MapPortrait>>(mapPortraits.JsonValue.ToString())!;
    }
}