using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OverwatchArcade.Domain.Models;
using OverwatchArcade.Domain.Models.ContributorInformation;
using OverwatchArcade.Domain.Models.Overwatch;
using OverwatchArcade.Persistence.Repositories.Interfaces;

namespace OverwatchArcade.Persistence.Repositories
{
    public class DailyRepository : Repository<Daily>, IDailyRepository
    {
        public DailyRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public Daily GetDaily()
        {
            return MUnitOfWork.Context.Dailies
                .Include(c => c.Contributor)
                .Include(c => c.TileModes)
                .ThenInclude(tile => tile.Label)
                .Include(c => c.TileModes)
                .ThenInclude(tile => tile.ArcadeMode)
                .OrderByDescending(d => d.Id)
                .First();
        }

        public async Task<int> GetContributedCount(Guid userId)
        {
            return await MUnitOfWork.Context.Dailies.Where(d => d.ContributorId == userId).CountAsync();
        }

        /// <summary>
        /// The contribution count of users in the v1 application
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetLegacyContributionCount(Guid userId)
        {
            var config = await MUnitOfWork.ConfigRepository.FirstOrDefaultASync(x => x.Key == ConfigKeys.V1ContributionCount.ToString());
            var contributions = JsonConvert.DeserializeObject<List<LegacyContributions>>(config?.JsonValue?.ToString() ?? string.Empty);

            var contributor = contributions?.Find(c => c.UserId.Equals(userId));
            return contributor?.Count ?? 0;
        }

        public async Task<DateTime> GetLastContribution(Guid userId)
        {
            var daily = await MUnitOfWork.Context.Dailies.Where(d => d.ContributorId == userId)
                .OrderByDescending(d => d.CreatedAt).FirstAsync();

            return daily.CreatedAt;
        }

        public string GetFavouriteContributionDay(Guid userId)
        {
            var query = MUnitOfWork.Context.Dailies
                .Where(p => p.ContributorId == userId)
                .AsEnumerable()
                .GroupBy(p => p.CreatedAt.DayOfWeek)
                .Select(g => new { day = g.Key, count = g.Count() }).ToList();

            return query[0].day.ToString();
        }

        public IEnumerable<DateTime> GetContributionDays(Guid userId)
        {
            return MUnitOfWork.Context.Dailies
                .Where(p => p.ContributorId == userId)
                .AsEnumerable()
                .Select(c => c.CreatedAt).ToList();
        }
        
        public async Task<bool> HasDailySubmittedToday()
        {
             return await MUnitOfWork.Context.Dailies
                .Where(d => d.MarkedOverwrite.Equals(false))
                .Where(d => d.CreatedAt >= DateTime.UtcNow.Date)
                .AnyAsync();
        }
    }
}