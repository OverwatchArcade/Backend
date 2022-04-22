using OWArcadeBackend.Models.Overwatch;
using System.Collections.Generic;
using OWArcadeBackend.Models.Constants;

namespace OWArcadeBackend.Persistence.Repositories.Interfaces
{
    public interface IOverwatchRepository : IRepository<ArcadeMode>
    {
        List<ArcadeMode> GetArcadeModes(Game gameType);
        List<Label> GetLabels();
    }
}
