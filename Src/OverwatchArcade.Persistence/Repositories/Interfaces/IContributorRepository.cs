using OverwatchArcade.Domain.Models;

namespace OverwatchArcade.Persistence.Repositories.Interfaces
{
    public interface IContributorRepository : IRepository<Contributor>
    {
        public Task<int> GetContributedCount(Guid userId);
        public Task<int> GetLegacyContributionCount(Guid userId);
        public Task<DateTime> GetLastContribution(Guid userId);
        public string GetFavouriteContributionDay(Guid userId);
        public IEnumerable<DateTime> GetContributionDays(Guid userId);
    }
}
