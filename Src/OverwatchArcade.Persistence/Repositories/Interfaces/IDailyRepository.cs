using OverwatchArcade.Domain.Models.Overwatch;

namespace OverwatchArcade.Persistence.Repositories.Interfaces
{
    public interface IDailyRepository : IRepository<Daily>
    {
        Daily GetDaily();
        public Task<int> GetContributedCount(Guid userId);
        public Task<int> GetLegacyContributionCount(Guid userId);
        public Task<DateTime> GetLastContribution(Guid userId);
        public string GetFavouriteContributionDay(Guid userId);
        public IEnumerable<DateTime> GetContributionDays(Guid userId);
        public Task<bool> HasDailySubmittedToday();
    }
}