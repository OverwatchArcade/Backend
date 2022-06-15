using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OverwatchArcade.Domain.Models.Overwatch;

namespace OverwatchArcade.Persistence.EntityConfiguration
{
    public class LabelConfiguration : IEntityTypeConfiguration<Label>
    {
        public void Configure(EntityTypeBuilder<Label> builder)
        {
            builder.HasData(
                    new Label { Id = 1, Value = null },
                    new Label { Id = 2, Value = "Daily" },
                    new Label { Id = 3, Value = "Weekly" }
                );
        }
    }
}
