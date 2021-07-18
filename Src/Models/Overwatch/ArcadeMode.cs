using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace OWArcadeBackend.Models.Overwatch
{
    public class ArcadeMode
    {
        [JsonIgnore]
        public int Id { get; set; }
        public Game Game { get; set; }

        [JsonProperty("Name"), Required]
        public string Name { get; set; }
        [Required]
        public string Players { get; set; }
        [JsonProperty("Description"), Required]
        public string Description { get; set; }
        public string Image { get; set; }
    }
}
