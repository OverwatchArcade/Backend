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
}