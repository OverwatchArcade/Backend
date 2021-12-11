using OWArcadeBackend.Dtos;
using OWArcadeBackend.Models.Overwatch;
using System.Collections.Generic;
using OWArcadeBackend.Dtos.Overwatch;
using OWArcadeBackend.Models;
using OWArcadeBackend.Models.Constants;

namespace OWArcadeBackend.Persistence.Repositories.Interfaces
{
    public interface IOverwatchRepository : IRepository<ArcadeMode>
    {
        List<ArcadeModeDto> GetArcadeModes(Game gameType);
        List<Label> GetLabels();
    }
}
