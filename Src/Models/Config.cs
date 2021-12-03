using System;
using Newtonsoft.Json.Linq;

namespace OWArcadeBackend.Models
{
    public class Config
    {
        public int Id { get; set; }
        public string Key { get; set; }
        #nullable enable
        public string? Value { get; set; }
        #nullable enable
        public JArray? JsonValue { get; set; }
    }

    public enum ConfigKeys
    {
        // GENERAL
        COUNTRIES,
        V1_CONTRIBUTION_COUNT,
        
        // OVERWATCH
        OW_TILES,
        OW_MAPS,
        OW_HEROES,
        OW_CURRENT_EVENT,
        OW_EVENTS,
        
        // OVERWATCH2 ?
        OW2_TILES
    }

    public class ConfigKeyValue
    {
        public string? Name { get; set; }
    }
    
    public class ConfigOverwatchMap : ConfigKeyValue
    {
        public string? Image { get; set; }
    }

    public class ConfigOverwatchHero : ConfigKeyValue 
    {
        public string? Image { get; set; }
    }
    
    public class ConfigCountries : ConfigKeyValue
    {
        // ISO 3166-1 Alpha-2
        public string? Code { get; set; }
    }

    public class ConfigV1Contributions : ConfigKeyValue
    {
        public Guid UserId { get; set; }
        public int Count { get; set; }
    }
    
}