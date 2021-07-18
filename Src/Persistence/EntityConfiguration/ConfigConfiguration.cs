using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OWArcadeBackend.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OWArcadeBackend.Persistence.EntityConfiguration
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

            using var heroReader = new StreamReader("Persistence/Seed/heroes.json");
            var jsonHeroesString = heroReader.ReadToEnd();
            var heroes = JArray.Parse(jsonHeroesString);

            using var mapReader = new StreamReader("Persistence/Seed/maps.json");
            var jsonMapString = mapReader.ReadToEnd();
            var maps = JArray.Parse(jsonMapString);

            using var countryReader = new StreamReader("Persistence/Seed/countries.json");
            var jsonCountryString = countryReader.ReadToEnd();
            var countries = JArray.Parse(jsonCountryString);

            var contribution = new List<ConfigV1Contributions>
            {
                new()
                {
                    UserId = Guid.Parse("e992ded4-30ca-4cdd-9047-d7f0a5ab6378"),
                    Count = 0
                }
            };

            builder.HasData(
                new Config {Id = 1, Key = ConfigKeys.COUNTRIES.ToString(), JsonValue = countries},
                new Config {Id = 2, Key = ConfigKeys.V1_CONTRIBUTION_COUNT.ToString(), JsonValue = JArray.Parse(JsonConvert.SerializeObject(contribution))},
                new Config {Id = 3, Key = ConfigKeys.OW_TILES.ToString(), Value = "7"},
                new Config {Id = 4, Key = ConfigKeys.OW_CURRENT_EVENT.ToString(), Value = "default"},
                new Config {Id = 5, Key = ConfigKeys.OW_MAPS.ToString(), JsonValue = maps},
                new Config {Id = 6, Key = ConfigKeys.OW_HEROES.ToString(), JsonValue = heroes},
                new Config {Id = 7, Key = ConfigKeys.OW2_TILES.ToString(), Value = "7"}
            );
        }
    }
}