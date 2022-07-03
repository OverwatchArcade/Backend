using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OverwatchArcade.Domain.Models.Overwatch;

namespace OverwatchArcade.Persistence.EntityConfiguration
{
    public class DailyConfiguration : IEntityTypeConfiguration<Daily>
    {
        public void Configure(EntityTypeBuilder<Daily> builder)
        {
            builder.HasMany(tm => tm.TileModes)
                .WithOne();

            builder.HasData(
                    new Daily { Id = 1, ContributorId = new Guid("e992ded4-30ca-4cdd-9047-d7f0a5ab6378"), CreatedAt = DateTime.Parse("01-01-2000") }
                );
        }
    }
}
