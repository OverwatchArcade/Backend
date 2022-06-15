using OverwatchArcade.Domain.Factories.interfaces;
using OverwatchArcade.Domain.Models.Constants;
using OverwatchArcade.Domain.Models.Overwatch;

namespace OverwatchArcade.Domain.Factories;

public class DailyFactory : IDailyFactory
{
    public Daily Create(Game game, Guid contributorId, ICollection<TileMode> tileModes)
    {
        return new Daily()
        {
            Game = game,
            ContributorId = contributorId,
            TileModes = tileModes
        };
    }
}