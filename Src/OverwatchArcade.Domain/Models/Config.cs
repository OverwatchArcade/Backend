using Newtonsoft.Json.Linq;

namespace OverwatchArcade.Domain.Models
{
    public class Config
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string? Value { get; set; }
        public JArray? JsonValue { get; set; }
    }

    public enum ConfigKeys
    {
        // GENERAL
        Countries,
        V1ContributionCount,
        
        // OVERWATCH
        OwTiles,
        OwMaps,
        OwHeroes,
        OwCurrentEvent,
        OwEvents,
    }
}