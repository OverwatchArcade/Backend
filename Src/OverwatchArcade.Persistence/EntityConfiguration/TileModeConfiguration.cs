using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OverwatchArcade.Domain.Models.Overwatch;

namespace OverwatchArcade.Persistence.EntityConfiguration
{
    public class TileModeConfiguration : IEntityTypeConfiguration<TileMode>
    {
        public void Configure(EntityTypeBuilder<TileMode> builder)
        {
            builder.HasKey(dm => new { dm.DailyId, dm.TileId });
            
            builder.HasOne(dm => dm.Label)
                   .WithMany()
                   .HasForeignKey(dm => dm.LabelId)
                   .OnDelete(DeleteBehavior.NoAction);
            
            builder.HasOne(am => am.ArcadeMode)
                .WithMany()
                .HasForeignKey(am => am.ArcadeModeId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasData(
                new TileMode { DailyId = 1, TileId = 1, ArcadeModeId = 1, LabelId = 1 },
                new TileMode { DailyId = 1, TileId = 2, ArcadeModeId = 1, LabelId = 2 },
                new TileMode { DailyId = 1, TileId = 3, ArcadeModeId = 1, LabelId = 3 },
                new TileMode { DailyId = 1, TileId = 4, ArcadeModeId = 1, LabelId = 1 },
                new TileMode { DailyId = 1, TileId = 5, ArcadeModeId = 1, LabelId = 2 },
                new TileMode { DailyId = 1, TileId = 6, ArcadeModeId = 1, LabelId = 3 },
                new TileMode { DailyId = 1, TileId = 7, ArcadeModeId = 1, LabelId = 1 },

                new TileMode { DailyId = 2, TileId = 1, ArcadeModeId = 1, LabelId = 1 },
                new TileMode { DailyId = 2, TileId = 2, ArcadeModeId = 1, LabelId = 2 },
                new TileMode { DailyId = 2, TileId = 3, ArcadeModeId = 1, LabelId = 3 },
                new TileMode { DailyId = 2, TileId = 4, ArcadeModeId = 1, LabelId = 1 },
                new TileMode { DailyId = 2, TileId = 5, ArcadeModeId = 1, LabelId = 2 },
                new TileMode { DailyId = 2, TileId = 6, ArcadeModeId = 1, LabelId = 3 },
                new TileMode { DailyId = 2, TileId = 7, ArcadeModeId = 1, LabelId = 1 }
            );
        }
    }
}
