using OWArcadeBackend.Models;
using OWArcadeBackend.Persistence.Repositories.Interfaces;

namespace OWArcadeBackend.Persistence.Repositories
{
    public class ContributorRepository : Repository<Contributor>, IContributorRepository
    {
        public ContributorRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}