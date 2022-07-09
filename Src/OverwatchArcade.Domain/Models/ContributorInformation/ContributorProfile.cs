using OverwatchArcade.Domain.Models.ContributorInformation.Game;

namespace OverwatchArcade.Domain.Models.ContributorInformation;

public class ContributorProfile
{
    public About Personal { get; init; }
    public Socials Social { get; set; }
    public OverwatchProfile Overwatch { get; init; }
}