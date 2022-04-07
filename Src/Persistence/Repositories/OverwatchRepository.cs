using OWArcadeBackend.Models.Overwatch;
using OWArcadeBackend.Persistence.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using OWArcadeBackend.Models.Constants;

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
