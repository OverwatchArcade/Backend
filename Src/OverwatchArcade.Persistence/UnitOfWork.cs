using Microsoft.Extensions.Logging;
using OverwatchArcade.Persistence.Repositories;
using OverwatchArcade.Persistence.Repositories.Interfaces;

namespace OverwatchArcade.Persistence
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        public AppDbContext Context { get; set; }
        private ILogger<UnitOfWork> Logger { get; }
        public IConfigRepository ConfigRepository { get; }
        public IContributorRepository ContributorRepository { get; }
        public IDailyRepository DailyRepository { get; }
        public ILabelRepository LabelRepository { get; }
        public IOverwatchRepository OverwatchRepository { get; }
        public IWhitelistRepository WhitelistRepository { get; }

        public UnitOfWork(AppDbContext context, ILogger<UnitOfWork> logger)
        {
            Context = context;
            Logger = logger;

            ConfigRepository = new ConfigRepository(this);
            ContributorRepository = new ContributorRepository(this);
            DailyRepository = new DailyRepository(this);
            LabelRepository = new LabelRepository(this);
            OverwatchRepository = new OverwatchRepository(this);
            WhitelistRepository = new WhitelistRepository(this);
        }
        
        public async Task Save()
        {
            try
            {
                Context.ChangeTracker.DetectChanges();
                await Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.LogWarning($"Error - Couldn't save changes {ex.Message}");
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Cleanup
        }
    }
}
