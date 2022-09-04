using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OverwatchArcade.Application.Common.Exceptions;
using OverwatchArcade.Application.Common.Interfaces;
using OverwatchArcade.Domain.Constants;
using OverwatchArcade.Domain.Entities.ContributorInformation.Game.Overwatch.Portraits;
using OverwatchArcade.Domain.Enums;

namespace OverwatchArcade.Application.Config.Queries.GetHeroPortraits;

public record GetHeroPortraitsQuery : IRequest<IEnumerable<HeroPortrait>>;

public class GetHeroPortraitsQueryHandler : IRequestHandler<GetHeroPortraitsQuery, IEnumerable<HeroPortrait>>
{
    private readonly IApplicationDbContext _context;

    public GetHeroPortraitsQueryHandler(IApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<HeroPortrait>> Handle(GetHeroPortraitsQuery request, CancellationToken cancellationToken)
    {
        var heroPortraits = await _context.Config
            .FirstOrDefaultAsync(c => c.Key.Equals(ConfigKeys.OwHeroes.ToString()), cancellationToken);
        if (heroPortraits?.JsonValue == null)
        {
            throw new ConfigNotFoundException($"Config {ConfigKeys.OwHeroes.ToString()} not found");
        }
        
        return JsonConvert.DeserializeObject<IEnumerable<HeroPortrait>>(heroPortraits.JsonValue.ToString())!;
    }
}