using OverwatchArcade.Persistence.Repositories.Interfaces;

namespace OverwatchArcade.Persistence
{
    public interface IUnitOfWork
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
