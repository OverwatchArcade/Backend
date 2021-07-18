using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OWArcadeBackend.Models;
using OWArcadeBackend.Models.Overwatch;

namespace OWArcadeBackend.Persistence.EntityConfiguration
{
    public class ArcadeModeConfiguration : IEntityTypeConfiguration<ArcadeMode>
    {
        public void Configure(EntityTypeBuilder<ArcadeMode> builder)
        {
            builder.HasData(
                new ArcadeMode { Id = 1, Game = Game.OVERWATCH, Name = "OverwatchArcade.Today", Players = "1v1", Description = "Placeholder", Image = "DA5533B1DF3F2DBF78F47A71B115BE43.jpg" },
                new ArcadeMode { Id = 2, Game = Game.OVERWATCH2, Name = "OverwatchArcade.Today", Players = "1v1", Description = "Placeholder", Image = "DA5533B1DF3F2DBF78F47A71B115BE43.jpg" });
        }
    }

}
