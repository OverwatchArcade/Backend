using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OWArcadeBackend.Dtos;
using OWArcadeBackend.Models.Overwatch;
using OWArcadeBackend.Persistence.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OWArcadeBackend.Models;

namespace OWArcadeBackend.Persistence.Repositories
{
    public class DailyRepository : Repository<Daily>, IDailyRepository
    {
        private readonly IMapper _mapper;

        public DailyRepository(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
        }

        public async Task<DailyDto> GetDaily(Game overwatchType)
        {
            Daily daily = MUnitOfWork.Context.Dailies
                .Where(d => d.Game == overwatchType && d.MarkedOverwrite.Equals(false))
                .Include(c => c.Contributor)
                .Include(c => c.TileModes)
                .ThenInclude(tile => tile.Label)
                .Include(c => c.TileModes)
                .ThenInclude(tile => tile.ArcadeMode)
                .OrderByDescending(d => d.Id)
                .First();
            DailyDto dailyDto = _mapper.Map<DailyDto>(daily);
            dailyDto.IsToday = await HasDailySubmittedToday(overwatchType, daily);

            return dailyDto;
        }

        public async Task<bool> HasDailySubmittedToday(Game gameType, Daily daily = null)
        {
            daily ??= await MUnitOfWork.Context.Dailies
                .OrderByDescending(d => d.Id)
                .Where(d => d.Game.Equals(gameType) && d.MarkedOverwrite.Equals(false))
                .FirstAsync();

            return (daily.CreatedAt >= DateTime.UtcNow.Date && !daily.MarkedOverwrite);
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
            var contributions = JsonConvert.DeserializeObject<List<ConfigV1Contributions>>(config.JsonValue.ToString());

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
                .Select(g => new {day = g.Key, count = g.Count()}).ToList();
            
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