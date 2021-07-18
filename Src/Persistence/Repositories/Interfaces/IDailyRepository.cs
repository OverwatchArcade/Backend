using System;
using OWArcadeBackend.Dtos;
using OWArcadeBackend.Models.Overwatch;
using System.Threading.Tasks;
using OWArcadeBackend.Models;

namespace OWArcadeBackend.Persistence.Repositories.Interfaces
{
    public interface IDailyRepository : IRepository<Daily>
    {
        Task<DailyDto> GetToday(Game gameType);
        Task<bool> HasDailySubmittedToday(Game gameType, Daily daily = null);
        public Task<int> GetContributedCount(Guid userId);
        public Task<DateTime> GetLastContribution(Guid userId);
        public string GetFavouriteContributionDay(Guid userId);
    }
}