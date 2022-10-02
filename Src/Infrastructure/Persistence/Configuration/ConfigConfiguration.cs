using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OverwatchArcade.Domain.Constants;
using OverwatchArcade.Domain.Entities;

namespace OverwatchArcade.Persistence.Persistence.Configuration
{
    public class ConfigConfiguration : IEntityTypeConfiguration<Config>
    {
        public void Configure(EntityTypeBuilder<Config> builder)
        {
            builder.HasIndex(c => c.Key)
                .IsUnique();
            
            builder.Property(p => p.JsonValue)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<JArray>(v) ?? new JArray());

            var buildDirectory  = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            
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
                // bluedog
                new(Guid.Parse("EFBC1951-ABAC-44F9-0C4D-08D94896AE8B"), 21),
                // Pandaa
                new(Guid.Parse("3B0F99E8-32D8-40C7-0C4E-08D94896AE8B"), 253),
                // Ender
                new(Guid.Parse("FA209D78-3A2D-430C-1031-08D948B83E4E"), 0),
                // Fonarik
                new(Guid.Parse("FCA436F9-72E2-4095-64FD-08D94A59FE19"), 263),
                // Redhawk
                new(Guid.Parse("8E7AF35E-7B0E-4FF6-05E8-08D94E14F5A1"), 196),
                // KVKH
                new(Guid.Parse("49D9A2C4-A6D9-4E04-1EF4-08D969575F7A"), 95)
            };

            builder.HasData(
                new Config {Id = 1, Key = ConfigKeys.Countries, JsonValue = countries},
                new Config {Id = 2, Key = ConfigKeys.V1ContributionCount, JsonValue = JArray.Parse(JsonConvert.SerializeObject(contribution))},
                new Config {Id = 3, Key = ConfigKeys.OwTiles, Value = "7"},
                new Config {Id = 4, Key = ConfigKeys.OwCurrentEvent, Value = "default"},
                new Config {Id = 5, Key = ConfigKeys.OwMaps, JsonValue = maps},
                new Config {Id = 6, Key = ConfigKeys.OwHeroes, JsonValue = heroes}
            );
        }
    }
} 