using OverwatchArcade.Domain.Models;
using OverwatchArcade.Persistence.Persistence.Repositories.Interfaces;
using OWArcadeBackend.Persistence.Repositories;

namespace OverwatchArcade.Persistence.Persistence.Repositories
{
    public class ConfigRepository : Repository<Config>, IConfigRepository
    {
        public ConfigRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
