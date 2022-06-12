using OverwatchArcade.Domain.Models;
using OverwatchArcade.Persistence.Persistence.Repositories.Interfaces;
using OWArcadeBackend.Persistence.Repositories;

namespace OverwatchArcade.Persistence.Persistence.Repositories
{
    public class ContributorRepository : Repository<Contributor>, IContributorRepository
    {
        public ContributorRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}