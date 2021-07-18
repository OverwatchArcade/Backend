using System;
using Newtonsoft.Json;
using OWArcadeBackend.Models;

namespace OWArcadeBackend.Dtos
{
    public class ContributorDto
    {
        public string Username { get; set; }
        public string Avatar { get; set; }
        public DateTime RegisteredAt { get; set; } 
        public string Group { get; set; }
        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
        public ContributorProfile Profile { get; set; }
        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
        public ContributorStats Stats { get; set; }
    }
}