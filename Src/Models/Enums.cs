using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OWArcadeBackend.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Game
    {
        OVERWATCH,
        OVERWATCH2
    }
    
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ContributorGroup
    {
        Contributor,
        SuperContributor,
        Developer
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum CacheKeys
    {
        OverwatchDaily,
        OverwatchEvent,
        OverwatchEvents,
        OverwatchLabels,
        OverwatchArcademodes,
        OverwatchMaps,
        OverwatchHeroes,
        
        Countries
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SocialProviders
    {
        Discord
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum EmailTypes
    {
        Signup
    }
}
