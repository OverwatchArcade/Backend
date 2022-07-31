using System.Reflection;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OverwatchArcade.Application.Common.Interfaces;
using OverwatchArcade.Domain.Entities;
using OverwatchArcade.Domain.Entities.Overwatch;

namespace OverwatchArcade.Persistence.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    protected ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }


    public DbSet<ArcadeMode> ArcadeModes { get; set; }
    public DbSet<Daily> Dailies { get; set; }
    public DbSet<Contributor> Contributors { get; set; }
    public DbSet<Config> Config { get; set; }
    public DbSet<Label> Labels { get; set; }
    public DbSet<Whitelist> Whitelist { get; set; }
    
    public async Task SaveASync(CancellationToken cancellationToken)
    {
        await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}
