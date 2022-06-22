using OverwatchArcade.Domain.Models.Constants;
using OverwatchArcade.Domain.Models.Overwatch;

namespace OverwatchArcade.Domain.Factories.interfaces;

public interface IDailyFactory
{
    public Daily Create(Game game, Guid contributorId, ICollection<TileMode> tileModes);
}