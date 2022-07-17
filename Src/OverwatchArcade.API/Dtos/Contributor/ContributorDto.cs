using System.Text.Json.Serialization;
using OverwatchArcade.Domain.Models.ContributorInformation;

namespace OverwatchArcade.API.Dtos.Contributor
{
    public class ContributorDto
    {
        public string Username { get; set; }
        public string Avatar { get; set; }
        public DateTime RegisteredAt { get; set; } 
        public string Group { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ContributorStatsDto? Stats { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ContributorProfile? Profile { get; set; }

        public void RemoveDetailedInformation()
        {
            if (Stats != null) Stats.ContributionDays = null;
            Profile = null;
        }
    }
}