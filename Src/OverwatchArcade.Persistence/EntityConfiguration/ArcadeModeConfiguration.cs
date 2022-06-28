using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OverwatchArcade.Domain.Models.Overwatch;

namespace OverwatchArcade.Persistence.EntityConfiguration
{
    public class ArcadeModeConfiguration : IEntityTypeConfiguration<ArcadeMode>
    {
        public void Configure(EntityTypeBuilder<ArcadeMode> builder)
        {
            builder.HasIndex(am =>  new { am.Name, am.Players }).IsUnique();

            builder.HasData(
                new ArcadeMode { Id = 1, Name = "OverwatchArcade.Today", Players = "1v1", Description = "Placeholder", Image = "DA5533B1DF3F2DBF78F47A71B115BE43.jpg" });
        }
    }

}
