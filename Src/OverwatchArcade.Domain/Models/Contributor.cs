using OverwatchArcade.Domain.Models.Constants;
using OverwatchArcade.Domain.Models.ContributorInformation;

namespace OverwatchArcade.Domain.Models
{
    public class Contributor
    {
        public Guid Id { get; init; }
        public string Username { get; init; }
        public string Email { get; init; }
        public string Avatar { get; set; }
        public ContributorGroup Group { get; init; }
        public ContributorStats? Stats { get; set; }
        public ContributorInformation.ContributorProfile? Profile { get; set; }
        public DateTime RegisteredAt { get; init; }
        
        public bool HasDefaultAvatar()
        {
            return Avatar.Equals(ImageConstants.DefaultAvatar);
        }
    }
}