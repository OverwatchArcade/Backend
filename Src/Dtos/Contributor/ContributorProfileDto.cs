using OWArcadeBackend.Dtos.Contributor.Profile;

namespace OWArcadeBackend.Dtos.Contributor
{
    public class ContributorProfileDto
    {
        public AboutDto Personal { get; init; }
        public SocialsDto Social { get; set; }
        public GamesDto Game { get; init; }
    }
}