using OWArcadeBackend.Models.Overwatch;
using OWArcadeBackend.Persistence.Repositories.Interfaces;

namespace OWArcadeBackend.Persistence.Repositories
{
    public class LabelRepository : Repository<Label>, ILabelRepository
    {
        public LabelRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
