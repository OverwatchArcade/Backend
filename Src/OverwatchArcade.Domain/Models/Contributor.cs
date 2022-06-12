using OverwatchArcade.Domain.Models.Constants;
using OverwatchArcade.Domain.Models.ContributorProfile;

namespace OverwatchArcade.Domain.Models
{
    public class Contributor
    {
        public Guid Id { get; init; }
        public string Username { get; init; }
        public string Email { get; init; }
        public string Avatar { get; set; }
        public ContributorGroup Group { get; init; }
        public DateTime RegisteredAt { get; init; }
        public ContributorInformation.ContributorProfile ContributorProfile { get; set; }
    }
}