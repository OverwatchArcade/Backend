using System.Collections.Generic;
using System.Linq;
using OverwatchArcade.Domain.Models.Constants;
using OverwatchArcade.Domain.Models.Overwatch;
using OverwatchArcade.Persistence.Persistence;
using OverwatchArcade.Persistence.Persistence.Repositories.Interfaces;

namespace OWArcadeBackend.Persistence.Repositories
{
    public class OverwatchRepository : Repository<ArcadeMode>, IOverwatchRepository
    {
        public OverwatchRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public List<ArcadeMode> GetArcadeModes(Game overwatchType)
        {
            return MUnitOfWork.Context.ArcadeModes.Where(mode => mode.Game == overwatchType).ToList();
        }
        
        public List<Label> GetLabels()
        {
            List<Label> labels = MUnitOfWork.Context.Labels.ToList();
            return labels;
        }
    }
}
