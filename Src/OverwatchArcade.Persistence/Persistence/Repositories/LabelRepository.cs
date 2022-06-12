using OverwatchArcade.Domain.Models.Overwatch;
using OverwatchArcade.Persistence.Persistence;
using OverwatchArcade.Persistence.Persistence.Repositories.Interfaces;

namespace OWArcadeBackend.Persistence.Repositories
{
    public class LabelRepository : Repository<Label>, ILabelRepository
    {
        public LabelRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
