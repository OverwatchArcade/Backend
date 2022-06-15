using OverwatchArcade.Domain.Models.Constants;
using OverwatchArcade.Domain.Models.Overwatch;

namespace OverwatchArcade.Persistence.Repositories.Interfaces
{
    public interface IOverwatchRepository : IRepository<ArcadeMode>
    {
        List<ArcadeMode> GetArcadeModes(Game gameType);
        List<Label> GetLabels();
    }
}
