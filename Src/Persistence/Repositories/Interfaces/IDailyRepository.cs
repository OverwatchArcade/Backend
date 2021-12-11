using System;
using System.Collections.Generic;
using OWArcadeBackend.Dtos;
using OWArcadeBackend.Models.Overwatch;
using System.Threading.Tasks;
using OWArcadeBackend.Dtos.Overwatch;
using OWArcadeBackend.Models;
using OWArcadeBackend.Models.Constants;

namespace OWArcadeBackend.Persistence.Repositories.Interfaces
{
    public interface IDailyRepository : IRepository<Daily>
    {
        Task<DailyDto> GetDaily(Game overwatchType);
        Task<bool> HasDailySubmittedToday(Game overwatchType, Daily daily = null);
        public Task<int> GetContributedCount(Guid userId);
        public Task<int> GetLegacyContributionCount(Guid userId);
        public Task<DateTime> GetLastContribution(Guid userId);
        public string GetFavouriteContributionDay(Guid userId);
        public IEnumerable<DateTime> GetContributionDays(Guid userId);
    }
}