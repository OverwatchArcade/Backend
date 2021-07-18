using OWArcadeBackend.Persistence.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace OWArcadeBackend.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        public AppDbContext Context { get; set; }
        public IConfigRepository ConfigRepository { get; }
        public IContributorRepository ContributorRepository { get; }
        public IDailyRepository DailyRepository { get; }
        public ILabelRepository LabelRepository { get; }
        public IOverwatchRepository OverwatchRepository { get; }
        public IWhitelistRepository WhitelistRepository { get; }
        Task Save();
    }
}
