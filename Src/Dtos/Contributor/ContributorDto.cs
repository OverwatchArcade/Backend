using System;
using Newtonsoft.Json;
using OWArcadeBackend.Dtos.Contributor.Stats;

namespace OWArcadeBackend.Dtos.Contributor
{
    public class ContributorDto
    {
        public string Username { get; set; }
        public string Avatar { get; set; }
        public DateTime RegisteredAt { get; set; } 
        public string Group { get; set; }
        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
        public ContributorProfileDto Profile { get; set; }
        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
        public ContributorStatsDto Stats { get; set; }
    }
}