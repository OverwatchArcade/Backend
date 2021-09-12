using OWArcadeBackend.Models;
using OWArcadeBackend.Persistence.Repositories.Interfaces;


namespace OWArcadeBackend.Persistence.Repositories
{
    public class AuthRepository : Repository<Contributor>, IAuthRepository
    {
        public AuthRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
