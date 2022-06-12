using OverwatchArcade.Domain.Models.ContributorProfile;

namespace OverwatchArcade.Domain.Models.ContributorInformation;

public class ContributorProfile
{
    public About Personal { get; init; }
    public Socials Social { get; set; }
    public Games Game { get; init; }
}