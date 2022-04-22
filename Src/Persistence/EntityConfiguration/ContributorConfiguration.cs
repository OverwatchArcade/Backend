using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using OWArcadeBackend.Dtos.Contributor;
using OWArcadeBackend.Models;

namespace OWArcadeBackend.Persistence.EntityConfiguration
{
    public class ContributorConfiguration : IEntityTypeConfiguration<Contributor>
    {
        public void Configure(EntityTypeBuilder<Contributor> builder)
        {
            builder.Property(user => user.Avatar)
                   .HasDefaultValue("default.jpg");
            
            builder.Property(p => p.Profile)
                    .HasConversion(v => JsonConvert.SerializeObject(v),
                           v => JsonConvert.DeserializeObject<ContributorProfileDto>(v));

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
