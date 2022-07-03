using OverwatchArcade.Domain.Models.Overwatch;
using OverwatchArcade.Persistence.Repositories.Interfaces;

namespace OverwatchArcade.Persistence.Repositories
{
    public class OverwatchRepository : Repository<ArcadeMode>, IOverwatchRepository
    {
        public OverwatchRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public List<ArcadeMode> GetArcadeModes()
        {
            return MUnitOfWork.Context.ArcadeModes.ToList();
        }
        
        public List<Label> GetLabels()
        {
            List<Label> labels = MUnitOfWork.Context.Labels.ToList();
            return labels;
        }
    }
}
