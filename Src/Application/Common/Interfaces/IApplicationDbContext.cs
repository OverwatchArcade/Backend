using Microsoft.EntityFrameworkCore;
using OverwatchArcade.Domain.Entities.Overwatch;

namespace OverwatchArcade.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    public DbSet<ArcadeMode> ArcadeModes { get; set; }
    public DbSet<Daily> Dailies { get; set; }
    public DbSet<Domain.Entities.Contributor> Contributors { get; set; }
    public DbSet<Domain.Entities.Config> Config { get; set; }
    public DbSet<Label> Labels { get; set; }
    public DbSet<Domain.Entities.Whitelist> Whitelist { get; set; }

    public Task SaveASync(CancellationToken cancellationToken);
}