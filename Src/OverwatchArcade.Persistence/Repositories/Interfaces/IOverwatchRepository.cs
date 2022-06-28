using OverwatchArcade.Domain.Models.Overwatch;

namespace OverwatchArcade.Persistence.Repositories.Interfaces
{
    public interface IOverwatchRepository : IRepository<ArcadeMode>
    {
        List<ArcadeMode> GetArcadeModes();
        List<Label> GetLabels();
    }
}
