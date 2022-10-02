using OverwatchArcade.Domain.Constants;
using OverwatchArcade.Domain.Entities.ContributorInformation;
using OverwatchArcade.Domain.Enums;

namespace OverwatchArcade.Domain.Entities
{
    public class Contributor
    {
        public Guid Id { get; init; }
        public string Username { get; init; }
        public string Email { get; init; }
        public string Avatar { get; set; }
        public ContributorGroup Group { get; init; }
        public ContributorStats? Stats { get; set; }
        public ContributorProfile? Profile { get; set; }
        public DateTime RegisteredAt { get; init; }

        public bool HasDefaultAvatar => Avatar.Equals(ImageConstants.DefaultAvatar);
    }
}