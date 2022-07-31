using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace OverwatchArcade.Domain.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum CacheKeys
{
    OverwatchDaily,
    OverwatchDailySubmit,
    OverwatchLabels,
    OverwatchArcadeModesDtos,
        
    ConfigOverwatchEvent,
    ConfigOverwatchEvents,
    ConfigOverwatchArcadeModes,
    ConfigOverwatchMaps,
    ConfigOverwatchHeroes,
        
    Countries
}