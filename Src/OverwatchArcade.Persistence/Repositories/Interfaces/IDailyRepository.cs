using OverwatchArcade.Domain.Models.Overwatch;

namespace OverwatchArcade.Persistence.Repositories.Interfaces
{
    public interface IDailyRepository : IRepository<Daily>
    {
        Daily GetDaily();
        public Task SoftDeleteDaily();
        public Task HardDeleteDaily();
        public Task<bool> HasDailySubmittedToday();
    }
}