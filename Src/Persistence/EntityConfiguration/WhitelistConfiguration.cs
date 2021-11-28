using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OWArcadeBackend.Models;

namespace OWArcadeBackend.Persistence.EntityConfiguration
{
    public class WhitelistConfiguration : IEntityTypeConfiguration<Whitelist>
    {
        public void Configure(EntityTypeBuilder<Whitelist> builder)
        {
            builder.HasKey(wl => wl.ProviderKey);
        }
    }
}