using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OWArcadeBackend.Persistence.Repositories;
using OWArcadeBackend.Persistence.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace OWArcadeBackend.Persistence
{
    public class UnitOfWork : IDisposable, IUnitOfWork
    {
        public AppDbContext Context { get; set; }
        internal ILogger<UnitOfWork> Logger { get; private set; }
        public IConfigRepository ConfigRepository { get; set; }
        public IContributorRepository ContributorRepository { get; set; }
        public IDailyRepository DailyRepository { get; set; }
        public ILabelRepository LabelRepository { get; set; }
        public IOverwatchRepository OverwatchRepository { get; set; }
        public IWhitelistRepository WhitelistRepository { get; set; }

        public UnitOfWork(AppDbContext context, IMapper mapper, ILogger<UnitOfWork> logger)
        {
            Context = context;
            Logger = logger;

            ConfigRepository = new ConfigRepository(this, mapper);
            ContributorRepository = new ContributorRepository(this, mapper);
            DailyRepository = new DailyRepository(this, mapper);
            LabelRepository = new LabelRepository(this, mapper);
            OverwatchRepository = new OverwatchRepository(this, mapper);
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
