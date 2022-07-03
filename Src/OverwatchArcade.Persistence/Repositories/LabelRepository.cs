using OverwatchArcade.Domain.Models.Overwatch;
using OverwatchArcade.Persistence.Repositories.Interfaces;

namespace OverwatchArcade.Persistence.Repositories
{
    public class LabelRepository : Repository<Label>, ILabelRepository
    {
        public LabelRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
