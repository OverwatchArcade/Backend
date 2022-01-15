using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OWArcadeBackend.Models.Constants
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
        OverwatchDailySubmit,
        OverwatchEvent,
        OverwatchEvents,
        OverwatchLabels,
        OverwatchArcadeModes,
        OverwatchMaps,
        OverwatchHeroes,
        
        Countries
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SocialProviders
    {
        Discord
    }
}
