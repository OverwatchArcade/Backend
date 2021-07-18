using OWArcadeBackend.Dtos;
using OWArcadeBackend.Models.Overwatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OWArcadeBackend.Models;

namespace OWArcadeBackend.Persistence.Repositories.Interfaces
{
    public interface IOverwatchRepository : IRepository<ArcadeMode>
    {
        List<ArcadeModeDto> GetArcadeModes(Game gameType);
        List<Label> GetLabels();
    }
}
