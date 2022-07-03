using OverwatchArcade.Domain.Factories.interfaces;
using OverwatchArcade.Domain.Models.Overwatch;

namespace OverwatchArcade.Domain.Factories;

public class DailyFactory : IDailyFactory
{
    public Daily Create(Guid contributorId, ICollection<TileMode> tileModes)
    {
        return new Daily()
        {
            ContributorId = contributorId,
            TileModes = tileModes
        };
    }
}