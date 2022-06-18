using OverwatchArcade.Domain.Models.ContributorInformation;

namespace OverwatchArcade.API.Dtos.Contributor
{
    public class ContributorDto
    {
        public string Username { get; set; }
        public string Avatar { get; set; }
        public DateTime RegisteredAt { get; set; } 
        public string Group { get; set; }
        public ContributorStats? Stats { get; set; }
        public ContributorProfile? Profile { get; set; }
    }
}