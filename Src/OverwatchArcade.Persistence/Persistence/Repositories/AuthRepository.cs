using OverwatchArcade.Domain.Models;
using OverwatchArcade.Persistence.Persistence.Repositories.Interfaces;
using OWArcadeBackend.Persistence.Repositories;

namespace OverwatchArcade.Persistence.Persistence.Repositories
{
    public class AuthRepository : Repository<Contributor>, IAuthRepository
    {
        public AuthRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
