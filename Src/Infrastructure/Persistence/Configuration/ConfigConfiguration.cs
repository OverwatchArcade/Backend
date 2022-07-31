using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OverwatchArcade.Domain.Entities;
using OverwatchArcade.Domain.Enums;

namespace OverwatchArcade.Persistence.Persistence.Configuration
{
    public class ConfigConfiguration : IEntityTypeConfiguration<Config>
    {
        public void Configure(EntityTypeBuilder<Config> builder)
        {
            builder.HasIndex(c => c.Key)
                .IsUnique();

            builder.Property(p => p.JsonValue)
                .HasConversion(v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<JArray>(v));
            
            var buildDirectory  = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            using var heroReader = new StreamReader(buildDirectory + "/Seed/heroes.json");
            var jsonHeroesString = heroReader.ReadToEnd();
            var heroes = JArray.Parse(jsonHeroesString);

            using var mapReader = new StreamReader(buildDirectory + "/Seed/maps.json");
            var jsonMapString = mapReader.ReadToEnd();
            var maps = JArray.Parse(jsonMapString);

            using var countryReader = new StreamReader(buildDirectory + "/Seed/countries.json");
            var jsonCountryString = countryReader.ReadToEnd();
            var countries = JArray.Parse(jsonCountryString);

            var contribution = new List<KeyValuePair<Guid, int>>
            {
                new(Guid.Parse("e992ded4-30ca-4cdd-9047-d7f0a5ab6378"), 0)
            };

            builder.HasData(
                new Config {Id = 1, Key = ConfigKeys.Countries.ToString(), JsonValue = countries},
                new Config {Id = 2, Key = ConfigKeys.V1ContributionCount.ToString(), JsonValue = JArray.Parse(JsonConvert.SerializeObject(contribution))},
                new Config {Id = 3, Key = ConfigKeys.OwTiles.ToString(), Value = "7"},
                new Config {Id = 4, Key = ConfigKeys.OwCurrentEvent.ToString(), Value = "default"},
                new Config {Id = 5, Key = ConfigKeys.OwMaps.ToString(), JsonValue = maps},
                new Config {Id = 6, Key = ConfigKeys.OwHeroes.ToString(), JsonValue = heroes}
            );
        }
    }
} 