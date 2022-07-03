using OverwatchArcade.Domain.Models;
using OverwatchArcade.Persistence.Repositories.Interfaces;

namespace OverwatchArcade.Persistence.Repositories
{
    public class ConfigRepository : Repository<Config>, IConfigRepository
    {
        public ConfigRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
