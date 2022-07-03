using Microsoft.EntityFrameworkCore;
using OverwatchArcade.Domain.Models;
using OverwatchArcade.Domain.Models.Overwatch;

namespace OverwatchArcade.Persistence
{
    public interface IAppDbContext
    {
        public DbSet<ArcadeMode> ArcadeModes { get; set; }
        public DbSet<Daily> Dailies { get; set; }
        public DbSet<Contributor> Contributors { get; set; }
        public DbSet<Config> Config { get; set; }
        public DbSet<Label> Labels { get; set; }
        public DbSet<Whitelist> Whitelist { get; set; }
    }
}