using AutoMapper;
using Microsoft.Extensions.Logging;
using OWArcadeBackend.Persistence.Repositories;
using OWArcadeBackend.Persistence.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace OWArcadeBackend.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        public AppDbContext Context { get; set; }
        private ILogger<UnitOfWork> Logger { get; }
        public IConfigRepository ConfigRepository { get; }
        public IContributorRepository ContributorRepository { get; }
        public IDailyRepository DailyRepository { get; }
        public ILabelRepository LabelRepository { get; }
        public IOverwatchRepository OverwatchRepository { get; }
        public IWhitelistRepository WhitelistRepository { get; }

        public UnitOfWork(AppDbContext context, IMapper mapper, ILogger<UnitOfWork> logger)
        {
            Context = context;
            Logger = logger;

            ConfigRepository = new ConfigRepository(this);
            ContributorRepository = new ContributorRepository(this);
            DailyRepository = new DailyRepository(this, mapper);
            LabelRepository = new LabelRepository(this);
            OverwatchRepository = new OverwatchRepository(this);
            WhitelistRepository = new WhitelistRepository(this);
        }

        public void Dispose()
        {
            if (Context != null)
            {
                Context.Dispose();
                Context = null;
            }
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
    }
}
