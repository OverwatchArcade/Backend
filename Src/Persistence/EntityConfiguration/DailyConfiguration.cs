using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OWArcadeBackend.Models.Overwatch;
using System;
using OWArcadeBackend.Models;

namespace OWArcadeBackend.Persistence.EntityConfiguration
{
    public class DailyConfiguration : IEntityTypeConfiguration<Daily>
    {
        public void Configure(EntityTypeBuilder<Daily> builder)
        {
            builder.HasMany(tm => tm.TileModes)
                .WithOne();

            builder.HasData(
                    new Daily { Id = 1, ContributorId = new Guid("e992ded4-30ca-4cdd-9047-d7f0a5ab6378"), Game = Game.OVERWATCH, CreatedAt = DateTime.Parse("01-01-2000") },
                    new Daily { Id = 2, ContributorId = new Guid("e992ded4-30ca-4cdd-9047-d7f0a5ab6378"), Game = Game.OVERWATCH2, CreatedAt = DateTime.Parse("01-01-2000") }
                );
        }
    }
}
