using OWArcadeBackend.Models;
using OWArcadeBackend.Persistence.Repositories.Interfaces;

namespace OWArcadeBackend.Persistence.Repositories
{
    public class ConfigRepository : Repository<Config>, IConfigRepository
    {
        public ConfigRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
