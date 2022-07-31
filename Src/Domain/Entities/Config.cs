using Newtonsoft.Json.Linq;

namespace OverwatchArcade.Domain.Entities
{
    public class Config
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string? Value { get; set; }
        public JArray? JsonValue { get; set; }
    }
}