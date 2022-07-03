using OverwatchArcade.Domain.Models;
using OverwatchArcade.Persistence.Repositories.Interfaces;

namespace OverwatchArcade.Persistence.Repositories
{
    public class ContributorRepository : Repository<Contributor>, IContributorRepository
    {
        public ContributorRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}