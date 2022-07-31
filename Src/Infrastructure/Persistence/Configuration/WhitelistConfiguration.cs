using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OverwatchArcade.Domain.Entities;

namespace OverwatchArcade.Persistence.Persistence.Configuration
{
    public class WhitelistConfiguration : IEntityTypeConfiguration<Whitelist>
    {
        public void Configure(EntityTypeBuilder<Whitelist> builder)
        {
            builder.HasKey(wl => wl.ProviderKey);
        }
    }
}