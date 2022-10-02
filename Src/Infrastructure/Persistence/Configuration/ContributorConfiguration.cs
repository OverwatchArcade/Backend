using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using OverwatchArcade.Domain.Entities;
using OverwatchArcade.Domain.Entities.ContributorInformation;

namespace OverwatchArcade.Persistence.Persistence.Configuration
{
    public class ContributorConfiguration : IEntityTypeConfiguration<Contributor>
    {
        public void Configure(EntityTypeBuilder<Contributor> builder)
        {
            builder.Property(user => user.Avatar)
                   .HasDefaultValue("default.jpg");
            
            builder.Property(p => p.Profile)
                    .HasConversion(
                        v => JsonConvert.SerializeObject(v),
                           v => JsonConvert.DeserializeObject<ContributorProfile>(v) ?? new ContributorProfile());
            
            builder.Property(p => p.Stats)
                .HasConversion(v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<ContributorStats>(v) ?? new ContributorStats());

            builder.HasData(
                new Contributor()
                {
                    Id = new Guid("e992ded4-30ca-4cdd-9047-d7f0a5ab6378"),
                    Username = "System",
                    Email = "system@overwatcharcade.today",
                });
        }
    }
}
