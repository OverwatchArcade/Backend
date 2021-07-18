using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OWArcadeBackend.Models;
using OWArcadeBackend.Models.Overwatch;

namespace OWArcadeBackend.Persistence.EntityConfiguration
{
    public class WhitelistConfiguration : IEntityTypeConfiguration<Whitelist>
    {
        public void Configure(EntityTypeBuilder<Whitelist> builder)
        {
            builder.HasKey(wl => wl.ProviderKey);

            // bluedog's discord
            // builder.HasData(
            //     new Whitelist {ProviderKey = "140276297187196928", Provider = SocialProviders.Discord}
            // );
        }
    }
}