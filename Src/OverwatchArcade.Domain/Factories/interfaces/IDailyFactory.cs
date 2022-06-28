using OverwatchArcade.Domain.Models.Overwatch;

namespace OverwatchArcade.Domain.Factories.interfaces;

public interface IDailyFactory
{
    public Daily Create(Guid contributorId, ICollection<TileMode> tileModes);
}