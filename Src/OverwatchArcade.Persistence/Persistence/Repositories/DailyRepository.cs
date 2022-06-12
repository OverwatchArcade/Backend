using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OverwatchArcade.Domain.Models;
using OverwatchArcade.Domain.Models.Constants;
using OverwatchArcade.Domain.Models.ContributorProfile;
using OverwatchArcade.Domain.Models.Overwatch;
using OverwatchArcade.Persistence.Persistence.Repositories.Interfaces;
using OWArcadeBackend.Persistence.Repositories;

namespace OverwatchArcade.Persistence.Persistence.Repositories
{
    public class DailyRepository : Repository<Daily>, IDailyRepository
    {
        public DailyRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public Daily GetDaily(Game overwatchType)
        {
            return MUnitOfWork.Context.Dailies
                .Where(d => d.Game == overwatchType)
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
            var count = await MUnitOfWork.Context.Dailies.Where(d => d.ContributorId == userId).CountAsync();
            return count;
        }

        /// <summary>
        /// The contribution count of users in the v1 application
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetLegacyContributionCount(Guid userId)
        {
            var config = await MUnitOfWork.ConfigRepository.SingleOrDefaultASync(x => x.Key == ConfigKeys.V1_CONTRIBUTION_COUNT.ToString());
            var contributions = JsonConvert.DeserializeObject<List<LegacyContributions>>(config.JsonValue?.ToString() ?? string.Empty);

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
    }
}