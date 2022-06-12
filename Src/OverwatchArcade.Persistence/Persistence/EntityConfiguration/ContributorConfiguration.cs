using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using OverwatchArcade.Domain.Models;
using OverwatchArcade.Domain.Models.ContributorInformation;
using OverwatchArcade.Domain.Models.ContributorProfile;

namespace OverwatchArcade.Persistence.Persistence.EntityConfiguration
{
    public class ContributorConfiguration : IEntityTypeConfiguration<Contributor>
    {
        public void Configure(EntityTypeBuilder<Contributor> builder)
        {
            builder.Property(user => user.Avatar)
                   .HasDefaultValue("default.jpg");
            
            builder.Property(p => p.ContributorProfile)
                    .HasConversion(v => JsonConvert.SerializeObject(v),
                           v => JsonConvert.DeserializeObject<ContributorProfile>(v) ?? new ContributorProfile());

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
