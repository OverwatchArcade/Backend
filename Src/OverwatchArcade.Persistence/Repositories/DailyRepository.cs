using Microsoft.EntityFrameworkCore;
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

        public async Task SoftDeleteDaily()
        {
            foreach (var dailyOwMode in MUnitOfWork.DailyRepository.Find(d => d.CreatedAt >= DateTime.UtcNow.Date))
            {
                dailyOwMode.MarkedOverwrite = true;
            }

            await MUnitOfWork.Save();
        }

        public async Task HardDeleteDaily()
        {
            var dailies = MUnitOfWork.DailyRepository.Find(d => d.CreatedAt >= DateTime.UtcNow.Date);
            MUnitOfWork.DailyRepository.RemoveRange(dailies);

            await MUnitOfWork.Save();
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