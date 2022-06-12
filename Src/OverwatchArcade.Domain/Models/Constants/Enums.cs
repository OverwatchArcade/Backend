﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OverwatchArcade.Domain.Models.Constants
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
        OverwatchLabels,
        OverwatchArcadeModes,
        
        ConfigOverwatchEvent,
        ConfigOverwatchEvents,
        ConfigOverwatchArcadeModes,
        ConfigOverwatchMaps,
        ConfigOverwatchHeroes,
        
        Countries
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SocialProviders
    {
        Discord
    }
}
