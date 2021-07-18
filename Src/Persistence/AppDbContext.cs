using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OWArcadeBackend.Models;
using OWArcadeBackend.Models.Overwatch;

namespace OWArcadeBackend.Persistence
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<ArcadeMode> ArcadeModes { get; set; }
        public DbSet<Daily> Dailies { get; set; }
        public DbSet<Contributor> Contributors { get; set; }
        public DbSet<Config> Config { get; set; }
        public DbSet<Label> Labels { get; set; }
        public DbSet<Whitelist> Whitelist { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(builder);
        }
    }
}